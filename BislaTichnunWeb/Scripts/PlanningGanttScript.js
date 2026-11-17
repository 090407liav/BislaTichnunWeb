/* ============================================================
   PlanningGanttScript.js - all planningGantt.aspx page logic
   Requires WebJScript.js (BISLAT) to be loaded BEFORE this file.
   ============================================================ */

(function () {
    "use strict";

    if (typeof BISLAT === "undefined") {
        if (window.console && console.error) {
            console.error(
                "PlanningGanttScript.js requires WebJScript.js " +
                "to be loaded first."
            );
        }
        return;
    }

    /* ---------------------------------------------------------
       Shared helpers (from WebJScript.js)
       --------------------------------------------------------- */

    var byId = BISLAT.byId;
    var queryAll = BISLAT.queryAll;
    var closestElement = BISLAT.closestElement;
    var ajax = BISLAT.ajax;
    var openModal = BISLAT.openModal;

    /* ---------------------------------------------------------
       Date helpers (only used by this page)
       --------------------------------------------------------- */

    function addDays(dateString, days) {
        if (!dateString) {
            return "";
        }

        var parts = dateString.split("-");
        var date = new Date(
            parseInt(parts[0], 10),
            parseInt(parts[1], 10) - 1,
            parseInt(parts[2], 10)
        );

        date.setDate(date.getDate() + days);

        return date.getFullYear() + "-" +
            pad2(date.getMonth() + 1) + "-" +
            pad2(date.getDate());
    }

    function pad2(value) {
        return value < 10 ? "0" + value : String(value);
    }

    function dayDiff(start, end) {
        if (!start || !end) {
            return 0;
        }

        var startParts = start.split("-");
        var endParts = end.split("-");

        var startDate = Date.UTC(
            parseInt(startParts[0], 10),
            parseInt(startParts[1], 10) - 1,
            parseInt(startParts[2], 10)
        );

        var endDate = Date.UTC(
            parseInt(endParts[0], 10),
            parseInt(endParts[1], 10) - 1,
            parseInt(endParts[2], 10)
        );

        return Math.round((endDate - startDate) / 86400000);
    }

    /* ---------------------------------------------------------
       Table search / filters / columns / selection
       --------------------------------------------------------- */

    var table;
    var searchBox;
    var selectedCount;
    var visibleCount;
    var selectAll;
    var selectVisibleButton;
    var clearFiltersButton;
    var columnsMenu;
    var columnsMenuButton;
    var columnsMenuContent;
    var columnsMenuOptions;

    var columnFilterStates = {};

    var allGafValues = [];
    var appliedGafValues = null;
    var temporaryGafValues = [];

    var storagePrefix = "PlanningGantt_";

    var columnDefinitions = [
        { key: "select", label: "בחירת שורות", fixed: true },
        { key: "gaf", label: "גף" },
        { key: "courseNum", label: "מספר קורס" },
        { key: "profession", label: "מקצוע" },
        { key: "courseName", label: "שם הקורס" },
        { key: "trainees", label: "חניכים" },
        { key: "startDate", label: "תאריך פתיחה" },
        { key: "endDate", label: "תאריך סיום" }
    ];

    var attributeNames = {
        gaf: "data-gaf",
        courseNum: "data-course-num",
        profession: "data-profession",
        courseName: "data-course-name",
        trainees: "data-trainees",
        startDate: "data-start-date",
        endDate: "data-end-date"
    };

    function getRows() {
        return table.querySelectorAll("tbody tr.course-row");
    }

    function getColumnCells(key) {
        return table.querySelectorAll('[data-col="' + key + '"]');
    }

    function getFilterValue(key) {
        var input = table.querySelector(
            '[data-filter-col="' + key + '"]'
        );

        return input ?
            input.value.toLowerCase().replace(/^\s+|\s+$/g, "") :
            "";
    }


    //FILTER FUNCTIONS
    function containsValue(values, value) {
        return values.indexOf(value) !== -1;
    }

    function collectColumnValues(key) {
        var rows = getRows();
        var values = [];
        var attribute = attributeNames[key];
        var value;
        var i;

        for (i = 0; i < rows.length; i++) {
            value = rows[i].getAttribute(attribute) || "";

            if (!containsValue(values, value)) {
                values.push(value);
            }
        }

        values.sort(function (a, b) {
            return a.localeCompare(b, "he");
        });

        return values;
    }


    function renderColumnOptions(key) {
        var state = columnFilterStates[key];

        if (!state) {
            return;
        }

        var search = state.search.value.toLowerCase()
            .replace(/^\s+|\s+$/g, "");

        var visibleOptions = 0;
        var checkedOptions = 0;
        var i;
        var value;
        var option;
        var checkbox;
        var label;

        state.options.innerHTML = "";

        for (i = 0; i < state.allValues.length; i++) {
            value = state.allValues[i];

            if (value.toLowerCase().indexOf(search) === -1) {
                continue;
            }

            visibleOptions++;

            option = document.createElement("label");
            option.className = "excel-filter-option";

            checkbox = document.createElement("input");
            checkbox.type = "checkbox";
            checkbox.setAttribute("data-filter-value", value);
            checkbox.checked = containsValue(
                state.temporaryValues,
                value
            );

            if (checkbox.checked) {
                checkedOptions++;
            }

            label = document.createElement("span");
            label.appendChild(document.createTextNode(
                value === "" ? "(ללא ערך)" : value
            ));

            option.appendChild(checkbox);
            option.appendChild(label);

            state.options.appendChild(option);
        }

        state.selectAll.checked =
            visibleOptions > 0 &&
            checkedOptions === visibleOptions;

        state.selectAll.indeterminate =
            checkedOptions > 0 &&
            checkedOptions < visibleOptions;
    }

    function openColumnFilter(key) {
        var state = columnFilterStates[key];

        state.temporaryValues = state.appliedValues === null
            ? state.allValues.slice(0)
            : state.appliedValues.slice(0);

        state.search.value = "";

        renderColumnOptions(key);

        state.menu.style.display = "block";
        state.search.focus();
    }

    function closeColumnFilter(key) {
        var state = columnFilterStates[key];

        if (state) {
            state.menu.style.display = "none";
        }
    }

    function applyColumnFilter(key) {
        var state = columnFilterStates[key];

        if (state.temporaryValues.length === state.allValues.length) {
            state.appliedValues = null;
        } else {
            state.appliedValues = state.temporaryValues.slice(0);
        }

        if (state.appliedValues === null) {
            state.button.classList.remove("filter-active");
        } else {
            state.button.classList.add("filter-active");
        }

        closeColumnFilter(key);
        applyFilters();
    }


    function setupColumnFilter(key) {
        var state = {
            button: byId(key + "FilterButton"),
            menu: byId(key + "FilterMenu"),
            search: byId(key + "FilterSearch"),
            selectAll: byId(key + "SelectAll"),
            options: byId(key + "FilterOptions"),
            apply: byId(key + "FilterApply"),
            cancel: byId(key + "FilterCancel"),

            allValues: [],
            appliedValues: null,
            temporaryValues: []
        };

        if (!state.button || !state.menu ||
            !state.search || !state.selectAll ||
            !state.options || !state.apply ||
            !state.cancel) {
            return;
        }

        state.allValues = collectColumnValues(key);
        state.temporaryValues = state.allValues.slice(0);

        columnFilterStates[key] = state;

        state.button.onclick = function (event) {
            event = event || window.event;

            if (event.stopPropagation) {
                event.stopPropagation();
            }

            if (state.menu.style.display === "block") {
                closeColumnFilter(key);
            } else {
                openColumnFilter(key);
            }

            return false;
        };

        state.menu.onclick = function (event) {
            event = event || window.event;

            if (event.stopPropagation) {
                event.stopPropagation();
            }
        };

        state.search.oninput = function () {
            renderColumnOptions(key);
        };

        state.options.onchange = function (event) {
            var target;
            var value;
            var index;

            event = event || window.event;
            target = event.target || event.srcElement;

            if (!target ||
                target.getAttribute("data-filter-value") === null) {
                return;
            }

            value = target.getAttribute("data-filter-value");
            index = state.temporaryValues.indexOf(value);

            if (target.checked && index === -1) {
                state.temporaryValues.push(value);
            } else if (!target.checked && index !== -1) {
                state.temporaryValues.splice(index, 1);
            }

            renderColumnOptions(key);
        };

        state.selectAll.onchange = function () {
            var search = state.search.value.toLowerCase()
                .replace(/^\s+|\s+$/g, "");

            var i;
            var value;
            var index;

            for (i = 0; i < state.allValues.length; i++) {
                value = state.allValues[i];

                if (value.toLowerCase().indexOf(search) === -1) {
                    continue;
                }

                index = state.temporaryValues.indexOf(value);

                if (state.selectAll.checked && index === -1) {
                    state.temporaryValues.push(value);
                } else if (!state.selectAll.checked && index !== -1) {
                    state.temporaryValues.splice(index, 1);
                }
            }

            renderColumnOptions(key);
        };

        state.apply.onclick = function () {
            applyColumnFilter(key);
        };

        state.cancel.onclick = function () {
            closeColumnFilter(key);
        };

        renderColumnOptions(key);
    }

    function setupColumnFilters() {
        var i;
        var key;

        for (i = 0; i < columnDefinitions.length; i++) {
            key = columnDefinitions[i].key;

            if (columnDefinitions[i].fixed) {
                continue;
            }

            setupColumnFilter(key);
        }
    }

    function rowMatches(row, globalTerm) {
        var text;
        var i;
        var key;
        var filter;
        var value;
        var state;

        if (globalTerm) {
            text = [
                row.getAttribute("data-gaf"),
                row.getAttribute("data-course-num"),
                row.getAttribute("data-profession"),
                row.getAttribute("data-course-name"),
                row.getAttribute("data-trainees"),
                row.getAttribute("data-start-date"),
                row.getAttribute("data-end-date")
            ].join(" ").toLowerCase();

            if (text.indexOf(globalTerm) === -1) {
                return false;
            }
        }

        for (i = 0; i < columnDefinitions.length; i++) {
            key = columnDefinitions[i].key;

            if (columnDefinitions[i].fixed) {
                continue;
            }

            value = row.getAttribute(attributeNames[key]) || "";

            // Multi-select column filter
            state = columnFilterStates[key];

            if (state && state.appliedValues !== null) {
                if (!containsValue(state.appliedValues, value)) {
                    return false;
                }
            }

            // Preserve existing text filters, if present
            filter = getFilterValue(key);

            if (filter && value.toLowerCase().indexOf(filter) === -1) {
                return false;
            }
        }

        return true;
    }

    function updateSelectionStatus() {
        var rows = getRows();
        var selected = 0;
        var visible = 0;
        var visibleSelected = 0;
        var i;
        var checkbox;
        var isVisible;

        for (i = 0; i < rows.length; i++) {
            isVisible = rows[i].style.display !== "none";
            checkbox = rows[i].querySelector(".row-select");

            if (checkbox && checkbox.checked) {
                selected++;

                if (isVisible) {
                    visibleSelected++;
                }
            }

            if (isVisible) {
                visible++;
            }

            if (checkbox) {
                if (checkbox.checked) {
                    rows[i].classList.add("selected-row");
                } else {
                    rows[i].classList.remove("selected-row");
                }
            }
        }

        if (selectedCount) {
            selectedCount.innerHTML = selected;
        }

        if (visibleCount) {
            visibleCount.innerHTML = " | מוצגים " + visible + " קורסים";
        }

        if (selectAll) {
            selectAll.checked =
                visible > 0 && visibleSelected === visible;

            selectAll.indeterminate =
                visibleSelected > 0 && visibleSelected < visible;
        }
    }

    function applyFilters() {
        var rows = getRows();
        var term = searchBox ?
            searchBox.value.toLowerCase().replace(/^\s+|\s+$/g, "") :
            "";

        var i;

        for (i = 0; i < rows.length; i++) {
            rows[i].style.display =
                rowMatches(rows[i], term) ? "" : "none";
        }

        updateSelectionStatus();
    }

    function setColumnVisibility(key, visible) {
        var cells = getColumnCells(key);
        var i;

        for (i = 0; i < cells.length; i++) {
            cells[i].style.display = visible ? "" : "none";
        }

        try {
            if (window.localStorage) {
                window.localStorage.setItem(
                    storagePrefix + "column_" + key,
                    visible ? "1" : "0"
                );
            }
        } catch (ignore) {
        }

        updateMonthHeader();
    }

    function updateMonthHeader() {
        var header = table.querySelector(".month-metadata-header");
        var visibleMetadataColumns = 0;
        var i;
        var definition;
        var cells;

        if (!header) {
            return;
        }

        for (i = 0; i < columnDefinitions.length; i++) {
            definition = columnDefinitions[i];

            if (definition.fixed) {
                visibleMetadataColumns++;
                continue;
            }

            cells = getColumnCells(definition.key);

            if (cells.length > 0 &&
                cells[0].style.display !== "none") {
                visibleMetadataColumns++;
            }
        }

        header.colSpan = visibleMetadataColumns;
        header.style.display =
            visibleMetadataColumns > 0 ? "" : "none";
    }

    function buildColumnsMenu() {
        var i;
        var definition;
        var label;
        var checkbox;
        var initialVisibility;

        columnsMenuOptions.innerHTML = "";

        for (i = 1; i < columnDefinitions.length; i++) {
            definition = columnDefinitions[i];

            label = document.createElement("label");
            label.className = "column-option";

            checkbox = document.createElement("input");
            checkbox.type = "checkbox";
            checkbox.setAttribute("data-column-toggle", definition.key);

            initialVisibility = true;

            try {
                if (window.localStorage) {
                    initialVisibility =
                        window.localStorage.getItem(
                            storagePrefix + "column_" + definition.key
                        ) !== "0";
                }
            } catch (ignore) {
                initialVisibility = true;
            }

            checkbox.checked = initialVisibility;
            label.appendChild(checkbox);
            label.appendChild(
                document.createTextNode(" " + definition.label)
            );

            columnsMenuOptions.appendChild(label);

            setColumnVisibility(definition.key, initialVisibility);
        }
    }

    function clearAllFilters() {
        var inputs = table.querySelectorAll(".column-filter");
        var i;
        var key;
        var state;

        if (searchBox) {
            searchBox.value = "";
        }

        for (i = 0; i < inputs.length; i++) {
            inputs[i].value = "";
        }

        for (key in columnFilterStates) {
            if (!columnFilterStates.hasOwnProperty(key)) {
                continue;
            }

            state = columnFilterStates[key];

            state.appliedValues = null;
            state.temporaryValues = state.allValues.slice(0);

            state.button.classList.remove("filter-active");

            state.search.value = "";
            closeColumnFilter(key);
            renderColumnOptions(key);
        }

        applyFilters();
    }
    function addRowSelectionHandlers() {
        var rows = getRows();
        var i;
        var checkbox;

        for (i = 0; i < rows.length; i++) {
            checkbox = rows[i].querySelector(".row-select");

            if (!checkbox) {
                continue;
            }

            checkbox.onclick = function (event) {
                event = event || window.event;

                if (event.stopPropagation) {
                    event.stopPropagation();
                }

                updateSelectionStatus();
            };

            checkbox.onchange = updateSelectionStatus;
        }
    }

    function setupColumnResizing() {
        var handles = table.querySelectorAll(".column-resize-handle");
        var i;

        for (i = 0; i < handles.length; i++) {
            handles[i].onmousedown = startResize;
        }
    }

    function startResize(event) {
        event = event || window.event;

        var handle = event.target || event.srcElement;
        var header = handle.parentNode;
        var key = header.getAttribute("data-col");
        var startX = event.clientX;
        var startWidth = header.offsetWidth;
        var minWidth = 55;
        var tableCells = getColumnCells(key);
        var j;

        if (event.preventDefault) {
            event.preventDefault();
        }

        document.onmousemove = function (moveEvent) {
            moveEvent = moveEvent || window.event;

            var width = startWidth + (startX - moveEvent.clientX);

            if (width < minWidth) {
                width = minWidth;
            }

            for (j = 0; j < tableCells.length; j++) {
                tableCells[j].style.width = width + "px";
                tableCells[j].style.minWidth = width + "px";
                tableCells[j].style.maxWidth = width + "px";
            }
        };

        document.onmouseup = function () {
            document.onmousemove = null;
            document.onmouseup = null;

            try {
                if (window.localStorage) {
                    window.localStorage.setItem(
                        storagePrefix + "width_" + key,
                        header.offsetWidth
                    );
                }
            } catch (ignore) {
            }
        };
    }

    function restoreColumnWidths() {
        var i;
        var definition;
        var width;
        var cells;
        var j;

        for (i = 0; i < columnDefinitions.length; i++) {
            definition = columnDefinitions[i];

            try {
                width = window.localStorage ?
                    window.localStorage.getItem(
                        storagePrefix + "width_" + definition.key
                    ) : null;
            } catch (ignore) {
                width = null;
            }

            if (!width || isNaN(parseInt(width, 10))) {
                continue;
            }

            cells = getColumnCells(definition.key);

            for (j = 0; j < cells.length; j++) {
                cells[j].style.width = parseInt(width, 10) + "px";
                cells[j].style.minWidth = parseInt(width, 10) + "px";
                cells[j].style.maxWidth = parseInt(width, 10) + "px";
            }
        }
    }

    function initializeTable() {
        table = byId("ganttTable");

        if (!table) {
            return;
        }

        searchBox = byId("ganttGlobalSearch");
        selectedCount = byId("selectedRowsCount");
        visibleCount = byId("visibleRowsCount");
        selectAll = byId("selectAllRows");
        selectVisibleButton = byId("selectVisibleRows");
        clearFiltersButton = byId("clearGanttFilters");
        columnsMenu = byId("columnsMenu");
        columnsMenuButton = byId("columnsMenuButton");
        columnsMenuContent = byId("columnsMenuContent");
        columnsMenuOptions = byId("columnsMenuOptions");

        if (!searchBox || !selectAll || !selectVisibleButton ||
            !clearFiltersButton || !columnsMenu ||
            !columnsMenuButton || !columnsMenuContent ||
            !columnsMenuOptions) {
            return;
        }

        searchBox.onkeyup = applyFilters;
        searchBox.oninput = applyFilters;

        var filters = table.querySelectorAll(".column-filter");
        var i;

        for (i = 0; i < filters.length; i++) {
            filters[i].onkeyup = applyFilters;
            filters[i].oninput = applyFilters;
        }

        selectAll.onclick = function () {
            var rows = getRows();
            var i;
            var checkbox;

            for (i = 0; i < rows.length; i++) {
                if (rows[i].style.display === "none") {
                    continue;
                }

                checkbox = rows[i].querySelector(".row-select");

                if (checkbox) {
                    checkbox.checked = selectAll.checked;
                }
            }

            updateSelectionStatus();
        };

        selectVisibleButton.onclick = function () {
            var rows = getRows();
            var i;
            var checkbox;

            for (i = 0; i < rows.length; i++) {
                if (rows[i].style.display === "none") {
                    continue;
                }

                checkbox = rows[i].querySelector(".row-select");

                if (checkbox) {
                    checkbox.checked = true;
                }
            }

            updateSelectionStatus();
        };

        clearFiltersButton.onclick = clearAllFilters;

        columnsMenuButton.onclick = function (event) {
            event = event || window.event;

            if (event.stopPropagation) {
                event.stopPropagation();
            }

            columnsMenuContent.style.display =
                columnsMenuContent.style.display === "block"
                    ? "none"
                    : "block";

            return false;
        };

        document.addEventListener("click", function (event) {
            var target = event.target || event.srcElement;

            if (target !== columnsMenuButton &&
                !columnsMenu.contains(target)) {
                columnsMenuContent.style.display = "none";
            }

            var key;

            for (key in columnFilterStates) {
                if (!columnFilterStates.hasOwnProperty(key)) {
                    continue;
                }

                var state = columnFilterStates[key];

                if (target !== state.button &&
                    !state.menu.contains(target)) {
                    closeColumnFilter(key);
                }
            }
        });

        columnsMenuOptions.onchange = function (event) {
            event = event || window.event;

            var target = event.target || event.srcElement;

            if (target && target.getAttribute("data-column-toggle")) {
                setColumnVisibility(
                    target.getAttribute("data-column-toggle"),
                    target.checked
                );
            }
        };

        buildColumnsMenu();
        restoreColumnWidths();
        setupColumnResizing();
        addRowSelectionHandlers();
        setupColumnFilters();
        applyFilters();
    }

    /* ---------------------------------------------------------
       Gantt: state
       --------------------------------------------------------- */

    var _viewMode = "planning";
    var _dragData = null;
    var _colorStorageKey = "BISLAT_GANTT_CELL_COLORS_V1";
    var _colorMenu = null;

    /* ---------------------------------------------------------
       Gantt: drag and drop
       --------------------------------------------------------- */

    function initBankChips() {
        var chips = queryAll(".bank-chip[draggable]");

        for (var i = 0; i < chips.length; i++) {
            (function (chip) {
                chip.addEventListener("dragstart", function () {
                    _dragData = {
                        type: "bank",
                        dutyTypeId: chip.getAttribute("data-dt-id"),
                        durationDays: parseInt(
                            chip.getAttribute("data-duration"), 10
                        ) || 7
                    };
                });

                chip.addEventListener("dragend", function () {
                    _dragData = null;
                });
            })(chips[i]);
        }
    }

    function initAssignmentChips() {
        var chips = queryAll('.chip[draggable="true"]');

        for (var i = 0; i < chips.length; i++) {
            (function (chip) {
                chip.addEventListener("dragstart", function (event) {
                    _dragData = {
                        type: "assignment",
                        assignmentId: chip.getAttribute("data-id"),
                        dutyTypeId: chip.getAttribute("data-dt-id"),
                        courseId: chip.getAttribute("data-course-id"),
                        startDate: chip.getAttribute("data-start"),
                        endDate: chip.getAttribute("data-end"),
                        durationDays: dayDiff(
                            chip.getAttribute("data-start"),
                            chip.getAttribute("data-end")
                        ),
                        notes: chip.getAttribute("data-notes") || ""
                    };

                    if (event.dataTransfer) {
                        event.dataTransfer.effectAllowed = "move";
                    }

                    chip.style.opacity = "0.4";
                });

                chip.addEventListener("dragend", function () {
                    chip.style.opacity = "";
                    _dragData = null;
                });
            })(chips[i]);
        }
    }

    function initDropCells() {
        var cells = queryAll(".gantt-cell[data-course-id]");

        for (var i = 0; i < cells.length; i++) {
            (function (cell) {
                cell.addEventListener("dragover", function (event) {
                    if (_viewMode !== "planning") {
                        return;
                    }

                    event.preventDefault();

                    if (event.dataTransfer) {
                        event.dataTransfer.dropEffect =
                            _dragData && _dragData.type === "bank" ?
                                "copy" : "move";
                    }

                    cell.classList.add("drag-over");
                });

                cell.addEventListener("dragleave", function () {
                    cell.classList.remove("drag-over");
                });

                cell.addEventListener("drop", function (event) {
                    event.preventDefault();
                    cell.classList.remove("drag-over");

                    if (!_dragData || _viewMode !== "planning") {
                        return;
                    }

                    var courseId = parseInt(
                        cell.getAttribute("data-course-id"), 10
                    );

                    var weekStart = cell.getAttribute("data-week-start");

                    if (_dragData.type === "bank") {
                        var endDate = addDays(
                            weekStart,
                            (_dragData.durationDays || 7) - 1
                        );

                        ajax(
                            "AssignmentHandler.ashx?action=create",
                            "POST",
                            {
                                courseId: courseId,
                                dutyTypeId: parseInt(_dragData.dutyTypeId, 10),
                                startDate: weekStart,
                                endDate: endDate,
                                mode: "planning",
                                notes: ""
                            },
                            function (ok) {
                                if (ok) {
                                    location.reload();
                                } else {
                                    alert("שגיאה בהוספת התורנות");
                                }
                            }
                        );
                    } else if (_dragData.type === "assignment") {
                        var duration = _dragData.durationDays || 0;

                        ajax(
                            "AssignmentHandler.ashx?action=update",
                            "POST",
                            {
                                id: parseInt(_dragData.assignmentId, 10),
                                courseId: courseId,
                                dutyTypeId: parseInt(_dragData.dutyTypeId, 10),
                                startDate: weekStart,
                                endDate: addDays(weekStart, duration),
                                mode: "planning",
                                notes: _dragData.notes || ""
                            },
                            function (ok) {
                                if (ok) {
                                    location.reload();
                                } else {
                                    alert("שגיאה בהעברת התורנות");
                                }
                            }
                        );
                    }

                    _dragData = null;
                });
            })(cells[i]);
        }
    }

    /* ---------------------------------------------------------
       Gantt: assignment modal
       --------------------------------------------------------- */

    function openAssignmentModal(
        id, courseId, courseName, startDate, endDate,
        dutyTypeId, notes, mode
    ) {
        var modal = byId("assignmentModal");

        if (!modal) {
            return;
        }

        modal.querySelector("#modal-title").textContent =
            id ? "עריכת תורנות" : "הוספת תורנות";

        modal.querySelector("#modal-course-name").textContent =
            courseName || "";

        modal.querySelector("#am-id").value = id || "";
        modal.querySelector("#am-course-id").value = courseId || "";
        modal.querySelector("#am-start").value = startDate || "";
        modal.querySelector("#am-end").value = endDate || "";
        modal.querySelector("#am-notes").value = notes || "";

        var radios = modal.querySelectorAll('input[name="am-dt"]');

        for (var i = 0; i < radios.length; i++) {
            var checked = dutyTypeId &&
                parseInt(radios[i].value, 10) === dutyTypeId;

            radios[i].checked = !!checked;

            if (radios[i].parentElement) {
                radios[i].parentElement.classList.toggle(
                    "selected", !!checked
                );
            }
        }

        modal.querySelector("#btn-delete").style.display =
            id ? "" : "none";

        modal.querySelector("#btn-confirm").style.display =
            (id && (!mode || mode === "planning")) ? "" : "none";

        openModal("assignmentModal");
    }

    function initCellClick() {
        var cells = queryAll(".gantt-cell[data-course-id]");

        for (var i = 0; i < cells.length; i++) {
            (function (cell) {
                cell.addEventListener("click", function (event) {
                    if (_viewMode !== "planning") {
                        return;
                    }

                    if (closestElement(event.target, ".chip")) {
                        return;
                    }

                    openAssignmentModal(
                        null,
                        parseInt(cell.getAttribute("data-course-id"), 10),
                        cell.getAttribute("data-course-name") || "",
                        cell.getAttribute("data-week-start"),
                        cell.getAttribute("data-week-end")
                    );
                });
            })(cells[i]);
        }
    }

    function initChipClick() {
        var chips = queryAll(".chip[data-id]");

        for (var i = 0; i < chips.length; i++) {
            (function (chip) {
                chip.addEventListener("click", function (event) {
                    event.stopPropagation();

                    if (_viewMode !== "planning") {
                        return;
                    }

                    openAssignmentModal(
                        parseInt(chip.getAttribute("data-id"), 10),
                        parseInt(chip.getAttribute("data-course-id"), 10),
                        "",
                        chip.getAttribute("data-start"),
                        chip.getAttribute("data-end"),
                        parseInt(chip.getAttribute("data-dt-id"), 10),
                        chip.getAttribute("data-notes") || "",
                        chip.getAttribute("data-mode") || "planning"
                    );
                });
            })(chips[i]);
        }
    }

    function initDutyTypeOptions() {
        document.addEventListener("click", function (event) {
            var option = closestElement(
                event.target, ".duty-type-option"
            );

            if (!option) {
                return;
            }

            var allOptions = queryAll(".duty-type-option");

            for (var i = 0; i < allOptions.length; i++) {
                allOptions[i].classList.remove("selected");
            }

            option.classList.add("selected");

            var radio = option.querySelector('input[type="radio"]');

            if (radio) {
                radio.checked = true;
            }
        });
    }

    window.saveAssignment = function () {
        var modal = byId("assignmentModal");

        if (!modal) {
            return;
        }

        var id = modal.querySelector("#am-id").value;
        var selectedRadio = modal.querySelector(
            'input[name="am-dt"]:checked'
        );

        var startDate = modal.querySelector("#am-start").value;
        var endDate = modal.querySelector("#am-end").value;

        if (!selectedRadio) {
            alert("בחר סוג תורנות");
            return;
        }

        if (!startDate || !endDate) {
            alert("יש לבחור תאריך התחלה ותאריך סיום");
            return;
        }

        if (endDate < startDate) {
            alert("תאריך הסיום לא יכול להיות לפני תאריך ההתחלה");
            return;
        }

        var data = {
            courseId: parseInt(
                modal.querySelector("#am-course-id").value, 10
            ),
            dutyTypeId: parseInt(selectedRadio.value, 10),
            startDate: startDate,
            endDate: endDate,
            mode: "planning",
            notes: modal.querySelector("#am-notes").value
        };

        var action = id ? "update" : "create";

        if (id) {
            data.id = parseInt(id, 10);
        }

        ajax(
            "AssignmentHandler.ashx?action=" + action,
            "POST",
            data,
            function (ok) {
                if (ok) {
                    location.reload();
                } else {
                    alert("שגיאה בשמירה");
                }
            }
        );
    };

    window.deleteAssignment = function () {
        var id = byId("am-id").value;

        if (!id || !confirm("למחוק תורנות זו?")) {
            return;
        }

        ajax(
            "AssignmentHandler.ashx?action=delete",
            "POST",
            { id: parseInt(id, 10) },
            function (ok) {
                if (ok) {
                    location.reload();
                } else {
                    alert("שגיאה במחיקה");
                }
            }
        );
    };

    window.confirmAssignment = function () {
        var id = byId("am-id").value;

        if (!id) {
            return;
        }

        ajax(
            "AssignmentHandler.ashx?action=confirm",
            "POST",
            { id: parseInt(id, 10) },
            function (ok) {
                if (ok) {
                    location.reload();
                } else {
                    alert("שגיאה באישור");
                }
            }
        );
    };

    /* ---------------------------------------------------------
       Gantt: planning / active mode
       --------------------------------------------------------- */

    function setViewMode(mode) {
        _viewMode = mode;

        var buttons = queryAll(".mode-btn");

        for (var i = 0; i < buttons.length; i++) {
            var buttonMode = buttons[i].getAttribute("data-mode");

            if (buttonMode) {
                buttons[i].classList.toggle(
                    "active-planning", buttonMode === "planning"
                );

                buttons[i].classList.toggle(
                    "active-active", buttonMode === "active"
                );
            }
        }

        var bank = byId("duty-bank");

        if (bank) {
            bank.style.display = mode === "planning" ? "" : "none";
        }

        var chips = queryAll(".chip");

        for (var j = 0; j < chips.length; j++) {
            chips[j].setAttribute(
                "draggable",
                mode === "planning" ? "true" : "false"
            );
        }

        var cells = queryAll(".gantt-cell");

        for (var k = 0; k < cells.length; k++) {
            cells[k].style.cursor =
                mode === "planning" ? "pointer" : "default";
        }

        var assignments = queryAll("[data-assignment-mode]");

        for (var m = 0; m < assignments.length; m++) {
            var assignmentMode =
                assignments[m].getAttribute("data-assignment-mode");

            assignments[m].style.display =
                (mode === "active" && assignmentMode !== "active") ?
                    "none" : "";
        }
    }

    // PlanningGantt.aspx calls BISLAT.setViewMode(...) from inline onclick
    BISLAT.setViewMode = setViewMode;

    /* ---------------------------------------------------------
       Gantt: cell colors
       Note: localStorage is per browser/user, not shared storage.
       --------------------------------------------------------- */

    function readSavedColors() {
        try {
            var value = localStorage.getItem(_colorStorageKey);
            return value ? JSON.parse(value) : {};
        } catch (e) {
            return {};
        }
    }

    function saveColors(colors) {
        try {
            localStorage.setItem(_colorStorageKey, JSON.stringify(colors));
        } catch (e) {
            alert("לא ניתן לשמור את הצבע בדפדפן זה");
        }
    }

    function getCellColorKey(cell) {
        return [
            cell.getAttribute("data-course-id") || "",
            cell.getAttribute("data-week-start") || ""
        ].join("|");
    }

    function applySavedCellColors() {
        var colors = readSavedColors();
        var cells = queryAll(".gantt-cell");

        for (var i = 0; i < cells.length; i++) {
            var key = getCellColorKey(cells[i]);

            if (colors[key]) {
                cells[i].style.backgroundColor = colors[key];
                cells[i].classList.add("manually-colored");
            }
        }
    }

    function createColorMenu() {
        if (_colorMenu) {
            return;
        }

        _colorMenu = document.createElement("div");
        _colorMenu.className = "gantt-color-menu";
        _colorMenu.innerHTML =
            '<div class="gantt-color-title">צבע תא</div>' +
            '<button type="button" data-color="#254c70" ' +
            'style="background:#254c70" title="כחול"></button>' +
            '<button type="button" data-color="#285a43" ' +
            'style="background:#285a43" title="ירוק"></button>' +
            '<button type="button" data-color="#70542b" ' +
            'style="background:#70542b" title="כתום"></button>' +
            '<button type="button" data-color="#693747" ' +
            'style="background:#693747" title="אדום"></button>' +
            '<button type="button" data-color="#57467a" ' +
            'style="background:#57467a" title="סגול"></button>' +
            '<button type="button" data-color="#0d1826" ' +
            'style="background:#0d1826" title="צבע ברירת מחדל"></button>' +
            '<button type="button" class="gantt-color-close">ביטול</button>';

        document.body.appendChild(_colorMenu);

        _colorMenu.addEventListener("click", function (event) {
            var button = closestElement(event.target, "button");

            if (!button) {
                return;
            }

            if (button.classList.contains("gantt-color-close")) {
                _colorMenu.style.display = "none";
                return;
            }

            if (!_colorMenu._targetCell) {
                return;
            }

            var cell = _colorMenu._targetCell;
            var key = getCellColorKey(cell);
            var color = button.getAttribute("data-color");
            var colors = readSavedColors();

            if (color === "#0d1826") {
                delete colors[key];
                cell.style.backgroundColor = "";
                cell.classList.remove("manually-colored");
            } else {
                colors[key] = color;
                cell.style.backgroundColor = color;
                cell.classList.add("manually-colored");
            }

            saveColors(colors);
            _colorMenu.style.display = "none";
        });
    }

    function initCellColoring() {
        createColorMenu();

        var cells = queryAll(".gantt-cell");

        for (var i = 0; i < cells.length; i++) {
            (function (cell) {
                cell.addEventListener("contextmenu", function (event) {
                    event.preventDefault();

                    _colorMenu._targetCell = cell;
                    _colorMenu.style.display = "block";

                    _colorMenu.style.left = event.pageX + "px";
                    _colorMenu.style.top = event.pageY + "px";
                });
            })(cells[i]);
        }

        document.addEventListener("click", function (event) {
            if (_colorMenu &&
                !_colorMenu.contains(event.target)) {
                _colorMenu.style.display = "none";
            }
        });

        applySavedCellColors();
    }

    /* ---------------------------------------------------------
       Initialize
       --------------------------------------------------------- */

    function initializeGantt() {
        initBankChips();
        initAssignmentChips();
        initDropCells();
        initCellClick();
        initChipClick();
        initCellColoring();
        initDutyTypeOptions();
    }

    function initialize() {
        if (!byId("ganttTable")) {
            return;
        }

        initializeTable();
        initializeGantt();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize);
    } else {
        initialize();
    }
})();