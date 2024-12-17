$(document).ready(function () {
    // Add smooth animation on form submit
    $("#submit-btn").on('click', function () {
        // Show loading animation or message
        $(this).text('Logging in...').prop('disabled', true);
    });

    // Optional: Add focus animations for the input fields
    $("input").on('focus', function () {
        $(this).css('border-color', '#003049');
    });

    $("input").on('blur', function () {
        $(this).css('border-color', '#ccc');
    });
});
