/**
 * notifications.js  —  Global SweetAlert2 helpers
 * Replaces all browser alert() calls across the site
 */

window.Notify = {
    // ✅ Success toast (top-end, auto-dismiss)
    success: function (msg) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'success',
            title: msg,
            showConfirmButton: false,
            timer: 3500,
            timerProgressBar: true,
            didOpen: function (toast) {
                toast.addEventListener('mouseenter', Swal.stopTimer);
                toast.addEventListener('mouseleave', Swal.resumeTimer);
            }
        });
    },

    // ❌ Error toast
    error: function (msg) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'error',
            title: msg,
            showConfirmButton: false,
            timer: 4500,
            timerProgressBar: true,
            didOpen: function (toast) {
                toast.addEventListener('mouseenter', Swal.stopTimer);
                toast.addEventListener('mouseleave', Swal.resumeTimer);
            }
        });
    },

    // ℹ️ Info toast
    info: function (msg) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'info',
            title: msg,
            showConfirmButton: false,
            timer: 3500,
            timerProgressBar: true,
            didOpen: function (toast) {
                toast.addEventListener('mouseenter', Swal.stopTimer);
                toast.addEventListener('mouseleave', Swal.resumeTimer);
            }
        });
    },

    // ⚠️ Warning toast
    warning: function (msg) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'warning',
            title: msg,
            showConfirmButton: false,
            timer: 4000,
            timerProgressBar: true,
            didOpen: function (toast) {
                toast.addEventListener('mouseenter', Swal.stopTimer);
                toast.addEventListener('mouseleave', Swal.resumeTimer);
            }
        });
    }
};
