/*Set thời gian hiển thị lỗi */
    document.addEventListener("DOMContentLoaded", function () {
        setTimeout(function () {
            var errorMessages = document.querySelector('.validation-summary-errors');
            if (errorMessages) {
                errorMessages.style.display = 'none';
            }
        }, 800); 
    });
