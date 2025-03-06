var dataTable;

$(document).ready(function () {
    loadDataTable();
})



function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Property/GetAll",
        },
        "pageLength": 5,
        "lengthMenu": [5, 10, 15, 20, 25],
        "columns": [
            { "data": "title"},
            { "data": "user.name"},
            { "data": "description"},
            { "data": "price"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                        <a href = "/Admin/Property/Upsert/${data}" class="btn btn-info"><i class="fa-regular fa-pen-to-square"></i></a>
                        <a onclick=Delete("/Admin/Property/Delete/${data}") class="btn btn-danger"><i class="fa-regular fa-trash-can"></i></a>
                        </div>
                    `;
                }
            }
        ]
    })
}


function Delete(url) {
    swal({
        title: "Want to delete the data ? ",
        text: "Delete Information !!!",
        icon: "warning",
        buttons: true,
        dangerModel: true
    }).then((wantDelete) => {
        if (wantDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                datatype: "json",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}