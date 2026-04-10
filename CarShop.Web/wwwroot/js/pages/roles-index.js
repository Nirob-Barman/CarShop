function confirmDeleteRole(btn, roleName) {
    Swal.fire({
        title: 'Delete role?', text: "Delete role '" + roleName + "'?", icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#dc3545', cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete', cancelButtonText: 'Cancel'
    }).then(function (result) { if (result.isConfirmed) btn.closest('form').submit(); });
}
