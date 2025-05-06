function validateEmailFormat(email) {
    // Regular expression for email format validation
    var emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailRegex.test(email); // Return true if valid, false otherwise
}
