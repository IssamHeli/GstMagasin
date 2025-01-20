function isInViewport(element) {
    var rect = element.getBoundingClientRect();
    return (
        rect.top <= (window.innerHeight || document.documentElement.clientHeight) &&
        rect.left >= 0 &&
        rect.bottom >= 0 &&
        rect.right <= (window.innerWidth || document.documentElement.clientWidth)
    );
}
function handleScrollAnimations() {
    var elements = document.querySelectorAll('.animate-scroll');

    elements.forEach(function (element) {
        if (isInViewport(element)) {
            element.classList.add('show');
        }
    });

}

window.addEventListener('scroll', handleScrollAnimations);

// Initial check on page load
window.addEventListener('load', handleScrollAnimations);


document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        document.querySelector(this.getAttribute('href')).scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
    });
});



document.addEventListener('scroll', function () {
    const sections = document.querySelectorAll('.fade-in1');
    sections.forEach(section => {
        const rect = section.getBoundingClientRect();
        if (rect.top <= window.innerHeight * 0.8) {
            section.classList.add('visible');
        }
    });

    let scrollToTopBtn = document.getElementById("scrollToTopBtn");

    // Show the button after 100px scroll
    if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
        scrollToTopBtn.classList.add("show");
    } else {
        scrollToTopBtn.classList.remove("show");
    }

});


let scrollToTopBtn = document.getElementById("scrollToTopBtn");


scrollToTopBtn.addEventListener('click', function () {
    window.scrollTo({ top: 0, behavior: 'smooth' });
});



$(document).ready(function () {
    if (localStorage.getItem('formSubmitted') === 'true') {
        // Disable the form or button if already submitted
        $('.codestatus').text("Vous avez déjà soumis votre message.").addClass('alert alert-info');

        $('#submitButton').prop('disabled', true); // Disable the submit button
    } else {
        // Enable the form submission if not submitted
        $('#submitButton').prop('disabled', false);
    }

        $('#submitButton').on('click', function (e) {
            e.preventDefault();

            
            const Email = $('#Email').val();
            const Name = $('#Name').val();
            const Message = $('#Message').val();

            // Check if the email input is empty

            if (Name.trim() === '') {
                $('.codestatus').text("Nom est obligatoire.").removeClass('alert-success').addClass('alert alert-danger');
                return; // Stop the submission
            }
            if (Email.trim() === '') {
                $('.codestatus').text("Email est obligatoire.").removeClass('alert-success').addClass('alert alert-danger');
                return; // Stop the submission
            }
            if (Message.trim() === '') {
                $('.codestatus').text("Message est obligatoire.").removeClass('alert-success').addClass('alert alert-danger');
                return; // Stop the submission
            }

            // Check if the email is in a valid format using a regular expression
            const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

            if (!emailPattern.test(Email)) {
                $('.codestatus').text("Veuillez entrer un email valide.").removeClass('alert-success').addClass('alert alert-danger');
                return; // Stop the submission
            }

            if (!$('input[name="RadioBtn"]').is(':checked')) {
                $('.codestatus').text("Vous devez accepter que Vous données soumises soient collectées et stockées.").addClass('alert alert-danger');;
                return; // Stop the request
            }

            $(this).text("Sending Email...").attr("disabled", true);


            grecaptcha.ready(function () {
                grecaptcha.execute('6LeGeqgqAAAAANsy_SLnKR6UmF4gMXghd5V97fCc', { action: 'UserContacterNous' }).then(function (token) {
                    // Add the token to the form before submitting
                    $('#recaptchaToken').val(token);

                    // Now submit the form using AJAX
                    $.ajax({
                        url: '/UserContact/UserContacterNous',
                        type: 'POST',
                        data: $('#formEmailSender').serialize(),
                        success: function (response) {
                            const codestatus = $('.codestatus');
                            if (response.status === "success") {
                                codestatus.text(response.message)
                                    .removeClass('alert-danger')
                                    .addClass('alert alert-success');

                                $('#formEmailSender')[0].reset(); // Reset form fields
                                // Optional: Deselect the radio button
                                $('input[name="RadioBtn"]').prop('checked', false);
                                $('#submitButton').text("Envoyer").attr("disabled", true);
                                localStorage.setItem('formSubmitted', 'true');
                            } else {
                                codestatus.text(response.message)
                                    .removeClass('alert-success')
                                    .addClass('alert alert-danger');
                                $('#submitButton').text("Envoyer").attr("disabled", false);
                            }
                        },
                        error: function () {
                            $('.codestatus').text("Erreur : Échec d'envoyer votre message.")
                                .removeClass('alert-success')
                                .addClass('alert alert-danger');
                            $('#submitButton').text("Envoyer").attr("disabled", false);
                        }
                    });
                });
            });
           
        });
    });



