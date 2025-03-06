var review;

$(document).ready(function () {
    loadReview();
});

// Load DataTable with Star Ratings
function loadReview() {
    review = $('#tblReview').DataTable({
        "ajax": {
            "url": "/Admin/Admin/GetReview",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                if (!json.data || json.data.length === 0) {
                    $("#tblReview tbody").html('<tr><td colspan="4" class="text-center">No reviews found.</td></tr>');
                    return [];
                }
                return json.data;
            },
            "error": function (xhr, error, thrown) {
                console.log("AJAX Error:", xhr.responseText);
            }
        },
        "columns": [
            { "data": "propertyTitle" },
            { "data": "userName" },
            { "data": "comment" },
            {
                "data": "rating",
                "render": function (data, type, row) {
                    return generateStars(data); // Call function to generate stars
                }
            }
        ],
        "destroy": true // Allows reloading the table without duplication
    });
}

// Function to generate star icons
function generateStars(rating) {
    let stars = "";
    for (let i = 1; i <= 5; i++) {
        if (i <= rating) {
            stars += '<i class="fas fa-star text-warning"></i>'; // Full Star (Yellow)
        } else {
            stars += '<i class="far fa-star text-muted"></i>'; // Empty Star (Gray)
        }
    }
    return stars;
}
