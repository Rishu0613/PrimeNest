var dataTable;

$(document).ready(function () {
    // Ensure DataTables is loaded
    if (!$.fn.DataTable) {
        console.error("DataTables is not loaded.");
        return;
    }
    loadDataTable();
});

function loadDataTable() {
    if ($.fn.DataTable.isDataTable("#tblData")) {
        $('#tblData').DataTable().destroy(); // Destroy existing instance
    }

    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Customer/Booking/GetAll",
            "type": "GET",
            "dataType": "json"
        },
        "processing": true,
        "serverSide": false,
        "pageLength": 5,
        "lengthMenu": [5, 10, 15, 20, 25],
        "columns": [
            { "data": "customerName" },
            { "data": "bookingDate" },
            { "data": "propertyName" }, // Fixed property reference
            { "data": "bookingTime" }
        ],
        "language": {
            "emptyTable": "No bookings found."
        }
    });
}
