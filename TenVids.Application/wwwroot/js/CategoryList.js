var categories = [];

$(function () {
    getCategories();
});

// HTML escaping/unescaping utilities
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

function unescapeHtml(safe) {
    return safe
        .replace(/&amp;/g, "&")
        .replace(/&lt;/g, "<")
        .replace(/&gt;/g, ">")
        .replace(/&quot;/g, '"')
        .replace(/&#039;/g, "'");
}

function sanitizeInput(input) {
    return input.replace(/<[^>]*>/g, "");
}

// Main data functions
function getCategories() {
    $.ajax({
        url: "/Admin/GetCategories",
        type: "GET",
        success: function (data) {
            if (data && data.success) {
                $('#tableBody').empty();
                categories = data.result;
                categories.forEach((category, index) => {
                    const trTag = `<tr id="trId_${index}" style="text-align:center"></tr>`;
                    $('#tableBody').append(trTag);
                    displayTableRow('display', index, category.id, category.name);
                });
            } else {
                showNotification("Error", data?.message || "Failed to load categories", "error");
            }
        },
        error: function (xhr) {
            console.error("Error loading categories:", xhr.responseText);
            showNotification("Error", "Failed to load categories. Please try again.", "error");
        }
    });
}

function displayTableRow(mode, index, id, name) {
    $('#trId_' + index).empty();

    const displayName = mode === 'display' ? escapeHtml(name) : name;
    const editValue = mode === 'edit' ? unescapeHtml(name) : name;

    let innerHtml = "";
    innerHtml += `<td style="width:10%; text-align:center">${id}</td>`;

    if (mode === 'display') {
        innerHtml += `<td style="width:10%; text-align:center">${displayName}</td>`;
        innerHtml += '<td style="width:15%; text-align:center">';
        innerHtml += `<button onclick="displayTableRow('edit', ${index}, ${id}, '${escapeHtml(name)}')" class="btn btn-warning me-2">`;
        innerHtml += '<i class="bi bi-pencil-square"></i> Edit</button>';
        innerHtml += `<button onclick="deleteRecord(${id}, '${escapeHtml(name)}')" class="btn btn-danger">`;
        innerHtml += '<i class="bi bi-trash"></i> Delete</button>';
        innerHtml += '</td>';
    } else {
        innerHtml += '<td style="width:10%">';
        innerHtml += `<input id="inputId_${index}" type="text" class="form-control" value="${editValue}" />`;
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
    const inputField = $(`#inputId_${index}`);
    const newName = sanitizeInput(inputField.val().trim());
    const validationSpan = $(`#validationTextId_${index}`);

    validationSpan.text(""); // Clear previous errors

    // Validation
    if (!newName) {
        validationSpan.text("Category name is required");
        inputField.focus();
        return;
    }

    $.ajax({
        url: "/Admin/AddEditCategory",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            Id: id,
            Name: newName
        }),
        success: function (response) {
            if (response && response.success) {
                showNotification("Success", "Category saved successfully", "success");
                // Refresh the data after a small delay
                setTimeout(() => getCategories(), 300);
            } else {
                validationSpan.text(response?.message || "Unknown error occurred");
            }
        },
        error: function (xhr) {
            const errorResponse = xhr.responseJSON;
            const errorMsg = errorResponse?.message ||
                errorResponse?.title ||
                "Error saving category";
            console.error("Error saving category:", errorMsg);
            validationSpan.text(errorMsg);
            showNotification("Error", errorMsg, "error");
        }
    });
}

function deleteRecord(id, name) {
    if (!confirm(`Are you sure you want to delete category '${unescapeHtml(name)}'?`)) {
        return;
    }

    $.ajax({
        url: `/Admin/DeleteCategory/${id}`,
        type: "DELETE",
        success: function (response) {
            if (response && response.success) {
                showNotification("Success", "Category deleted successfully", "success");
                getCategories();
            } else {
                showNotification("Error", response?.message || "Failed to delete category", "error");
            }
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON?.message || "Error deleting category";
            console.error("Error deleting category:", errorMsg);
            showNotification("Error", errorMsg, "error");
        }
    });
}

function showNotification(title, message, type) {
    // Using Toastr as an example - adjust to your notification system
    if (typeof toastr !== 'undefined') {
        toastr[type.toLowerCase()](message, title);
    } else {
        alert(`${title}: ${message}`);
    }
}