var dataTableAgents;
var dataTableIndividuals;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTableAgents = $('#tblAgents').DataTable({
        "ajax": {
            "url": "/Admin/Admin/GetAll",
            "dataSrc": "agents"
        },
        "columns": [
            {
                "data": "profilePic",
                "render": function (data) {
                    return `<img src="${data}" alt="Profile Picture" class="img-thumbnail" style="width:80px; height:50px; border-radius:40px;">`;
                },
                "width": "10%"
            },
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    return getLockUnlockButton(data);
                },
                "width": "15%"
            }
        ]
    });

    dataTableIndividuals = $('#tblIndividuals').DataTable({
        "ajax": {
            "url": "/Admin/Admin/GetAll",
            "dataSrc": "individuals"
        },
        "columns": [
            {
                "data": "profilePic",
                "render": function (data) {
                    return `<img src="${data}" alt="Profile Picture" class="img-thumbnail" style="width:80px; height:50px; border-radius:40px;">`;
                },
                "width": "10%"
            },
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    return getLockUnlockButton(data);
                },
                "width": "15%"
            } 
        ]
    });
}

function getLockUnlockButton(data) {
    var today = new Date().getTime();
    var lockOut = new Date(data.lockoutEnd).getTime();
    if (lockOut > today) {
        return `<div class="text-center">
                  <a class="btn btn-danger" onclick="lockunlock('${data.id}')">Unlock</a>
                </div>`;
    } else {
        return `<div class="text-center">
                  <a class="btn btn-success" onclick="lockunlock('${data.id}')">Lock</a>
                </div>`;
    }
}

function lockunlock(Id) {
    $.ajax({
        url: "/Admin/Admin/LockUnLock",
        type: "POST",
        data: JSON.stringify(Id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTableAgents.ajax.reload();
                dataTableIndividuals.ajax.reload();
            } else {
                toastr.error(data.message);
            }
        }
    });
}
