$(document).ready(function () {
    $(document).on('click', '.delete-button', function () {
        var id = $(this).data('id');
        var url = $(this).data('url'); // Get the URL from the data attribute

        Swal.fire({
            title: 'Bạn có chắc chắn muốn xóa?',
            text: "Hành động này không thể hoàn tác!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Xóa',
            cancelButtonText: 'Hủy',
            reverseButtons: false // Đảo ngược thứ tự của nút
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: { id: id },
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                title: 'Đã xóa!',
                                icon: 'success',
                                showConfirmButton: false // Ẩn nút "OK"
                            });

                            setTimeout(function () {
                                window.location.reload(); // Tải lại trang
                            }, 800); // 800 milliseconds
                        } else {
                            Swal.fire('Lỗi!', response.message, 'error');
                        }
                    },
                    error: function () {
                        Swal.fire('Lỗi!', 'Có lỗi xảy ra khi xóa.', 'error');
                    }
                });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                Swal.fire({
                    title: 'Hủy bỏ',
                    text: 'Xóa đã bị hủy bỏ',
                    icon: 'info',
                    showConfirmButton: false // Ẩn nút "OK"
                });
                setTimeout(function () {
                    Swal.close(); // Đóng cửa sổ thông báo
                }, 800); // 800 milliseconds
            }
        });
    });
});
