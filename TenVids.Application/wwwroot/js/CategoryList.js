var categories = [];

$(function () {
    getCategories();
});

function getCategories() {
    $.ajax({
        url: "/Admin/GetCategories",
        type: "GET",
        success: function (data) {
            $('#tableBody').empty();
            categories = data.result;
            categories.forEach((category, index) => {
                const trTag = `<tr id="trId_${index}" style="text-align:center"></tr>`;
                $('#tableBody').append(trTag);
                displayTableRow('display', index, category.id, category.name);
            });
        },
        error: function (xhr) {
            console.error("Error loading categories:", xhr.responseText);
            showNotification("Error", "Failed to load categories. Please try again.", "error");
        }
    });
}

function displayTableRow(mode, index, id, name) {
    $('#trId_' + index).empty();

    let innerHtml = "";
    innerHtml += `<td style="width:10%; text-align:center">${id}</td>`;

    if (mode === 'display') {
        innerHtml += `<td style="width:10%; text-align:center">${name}</td>`;
        innerHtml += '<td style="width:15%; text-align:center">';
        innerHtml += `<button onclick="displayTableRow('edit', ${index}, ${id}, '${escapeHtml(name)}')" class="btn btn-warning me-2">`;
        innerHtml += '<i class="bi bi-pencil-square"></i> Edit</button>';
        innerHtml += `<button onclick="deleteRecord(${id}, '${escapeHtml(name)}')" class="btn btn-danger">`;
        innerHtml += '<i class="bi bi-trash"></i> Delete</button>';
        innerHtml += '</td>';
    } else {
        innerHtml += '<td style="width:10%">';
        innerHtml += `<input id="inputId_${index}" type="text" class="form-control" value="${escapeHtml(name)}" />`;
        innerHtml += `<span class="text-danger" id="validationTextId_${index}"></span>`;
        innerHtml += '</td>';
        innerHtml += '<td style="width:15%; text-align:center">';
        innerHtml += `<button onclick="saveChanges(${index}, ${id})" class="btn btn-success me-2">`;
        innerHtml += '<i class="bi bi-check-circle"></i> Save</button>';
        innerHtml += `<button onclick="displayTableRow('display', ${index}, ${id}, '${escapeHtml(name)}')" class="btn btn-danger">`;
        innerHtml += '<i class="bi bi-x-circle"></i> Cancel</button>';
        innerHtml += '</td>';
    }

    $('#trId_' + index).append(innerHtml);
}

function saveChanges(index, id) {
    const newName = $(`#inputId_${index}`).val().trim();
    const validationSpan = $(`#validationTextId_${index}`);

    // Validation
    if (!newName) {
        validationSpan.text("Category name is required");
        return;
    }

    $.ajax({
        url: "/Admin/UpdateCategory",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ Id: id, Name: newName }),
        success: function () {
            displayTableRow('display', index, id, newName);
            showNotification("Success", "Category updated successfully", "success");
        },
        error: function (xhr) {
            console.error("Error updating category:", xhr.responseText);
            validationSpan.text(xhr.responseJSON?.message || "Error updating category");
        }
    });
}

function deleteRecord(id, name) {
    if (!confirm(`Are you sure you want to delete category '${name}'?`)) {
        return;
    }

    $.ajax({
        url: `/Admin/DeleteCategory/${id}`,
        type: "DELETE",
        success: function () {
            showNotification("Success", "Category deleted successfully", "success");
            getCategories(); 
        },
        error: function (xhr) {
            console.error("Error deleting category:", xhr.responseText);
            showNotification("Error", xhr.responseJSON?.message || "Error deleting category", "error");
        }
    });
}

function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

function showNotification(title, message, type) {
    // Implement your notification system here
    // Could use Toastr, SweetAlert, or your custom modal
    alert(`${title}: ${message}`); // Temporary basic alert
}