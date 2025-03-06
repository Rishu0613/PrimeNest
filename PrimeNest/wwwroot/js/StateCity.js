var stateTable;
var cityTable;

$(document).ready(function () {
    loadStateTable();
    loadCityTable(); // Call the city table loading function
});

// Load State DataTable
function loadStateTable() {
    stateTable = $('#tblStates').DataTable({
        "ajax": {
            "url": "/Admin/Admin/GetallState",
            "type": "GET",
            "datatype": "json",
            "dataSrc": "data",
            "error": function (xhr, error, thrown) {
                console.log("AJAX Error:", xhr.responseText);
            }
        },
        "columns": [
            { "data": "name" },
            {
                "data": "id",
                "title": "Actions",
                "render": function (data) {
                    return `<div class="text-center">
                    <a href="/Admin/Admin/State/${data}" class="btn btn-info"><i class="fa-regular fa-pen-to-square"></i></a>
                    <a onclick="Delete('/Admin/Admin/State/Delete/${data}')" class="btn btn-danger"><i class="fa-regular fa-trash-can"></i></a>
                </div>`;
                }
            }
        ]
    });
}

// Load City DataTable
function loadCityTable() {
    cityTable = $('#tblcity').DataTable({
        "ajax": {
            "url": "/Admin/Admin/GetAllCity",
            "type": "GET",
            "datatype": "json",
            "dataSrc": "data",
            "error": function (xhr, error, thrown) {
                console.log("AJAX Error:", xhr.responseText);
            }
        },
        "columns": [
            { "data": "name" },
            { "data": "state.name" },
            { "data": "zipCode" },
            {
                "data": "id",
                "title": "Actions",
                "render": function (data) {
                    return `<div class="text-center">
                    <a href="/Admin/Admin/City/${data}" class="btn btn-info"><i class="fa-regular fa-pen-to-square"></i></a>
                    <a onclick="Delete('/Admin/Admin/City/Delete/${data}')" class="btn btn-danger"><i class="fa-regular fa-trash-can"></i></a>
                </div>`;
                }
            }
        ]
    });
}

// Delete Function
function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "You are about to delete this entry!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((confirmDelete) => {
        if (confirmDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                datatype: "json",
                success: function (data) {
                    if (data.success) {
                        stateTable.ajax.reload();
                        cityTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Delete error:", xhr.responseText);
                }
            });
        }
    });
}
