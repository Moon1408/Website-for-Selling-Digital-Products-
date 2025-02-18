//// Thêm vào giỏ hàng
//// Của trang HangHoa/ Index
//$(document).on('click', '.ajax-add-to-cart', function () {
//    $.ajax({
//            url: "/Cart/AddToCart",
//            data: {
//                id: $(this).data("id"),
//                SoLuong: 1,
//                type: "ajax"
//            },
//            success: function (data) {
//                Swal.fire({
//                    position: 'top',
//                    icon: 'success',
//                    title: 'Thêm giỏ hàng thành công',
//                    showConfirmButton: false,
//                    timer: 2000
//                });
//                console.log(data.soLuong);
//                $("#cart_count").html(data.soLuong);
//            },
//            error: function () {
//                Swal.fire({
//                    position: 'top',
//                    icon: 'error',
//                    title: 'Thêm giỏ hàng thất bại',
//                    text: 'Vui lòng thử lại',
//                    showConfirmButton: false,
//                    timer: 2000
//                });
//            }
//        });
//    });
///*});*/

//// Thêm vào giỏ hàng
//// Của trang HangHoa/ Detail
//$(document).on('submit', '.ajax-add-to-cart1', function (e) {
//    e.preventDefault();
//    var form = $(this);
//    var id = form.data('id');
//    var soluong = form.find('input[name="SoLuong"]').val();

//    $.ajax({
//        url: "/Cart/AddToCart",
//        type: "POST",
//        data: {
//            id: id,
//            SoLuong: soluong,
//            type: "ajax"
//        },
//        success: function (data) {
//            Swal.fire({
//                position: 'top',
//                icon: 'success',
//                title: 'Thêm giỏ hàng thành công',
//                showConfirmButton: false,
//                timer: 2000
//            });
//            $("#cart_count").html(data.soLuong);
//        },
//        error: function () {
//            Swal.fire({
//                position: 'top',
//                icon: 'error',
//                title: 'Thêm giỏ hàng thất bại',
//                text: 'Vui lòng thử lại',
//                showConfirmButton: false,
//                timer: 2000
//            });
//        }
//    });
//});

function addToCart(id, quantity) {
    // Thêm vào giỏ hàng 
    $.ajax({
        url: "/Cart/AddToCart",
        type: "POST",
        data: {
            id: id,
            SoLuong: quantity,
            type: "ajax"
        },
        success: function (data) {
            Swal.fire({
                position: 'top',
                icon: 'success',
                title: 'Thêm giỏ hàng thành công',
                showConfirmButton: false,
                timer: 2000
            });
            $("#cart_count").html(data.soLuong);
        },
        error: function () {
                    Swal.fire({
                        position: 'top',
                        icon: 'error',
                        title: 'Thêm giỏ hàng thất bại',
                        text: 'Sản phẩm tạm thời hết hàng',
                        showConfirmButton: false,
                        timer: 2000
                    });
        }
    });
}
// Trang HangHoa/Index
$(document).on('click', '.ajax-add-to-cart', function () {
    addToCart($(this).data("id"), 1);
});

// Trang HangHoa/Detail
$(document).on('submit', '.ajax-add-to-cart1', function (e) {
    e.preventDefault();
    var form = $(this);
    var id = form.data('id');
    var soluong = form.find('input[name="SoLuong"]').val();
    addToCart(id, soluong);
});

