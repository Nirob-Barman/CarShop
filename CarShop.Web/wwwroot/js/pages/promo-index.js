function confirmToggle(btn, code, isActive) {
    var action = isActive ? 'Deactivate' : 'Activate';
    Swal.fire({
        title: action + ' "' + code + '"?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: isActive ? '#ffc107' : '#198754',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, ' + action.toLowerCase() + ' it'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}

function confirmDelete(btn, code) {
    Swal.fire({
        title: 'Delete "' + code + '"?',
        text: 'This cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}
