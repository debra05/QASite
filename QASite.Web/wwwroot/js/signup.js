$(() => {
    $("#email").on("blur", function () {
        console.log("foo");
        const email = $("#email").val();
        $.get(`/account/EmailExists?email=${email}`, function (obj) {
            if (!obj.emailExists) {
                $("#sign-up").prop('disabled', false);
            }
        });
    });
});