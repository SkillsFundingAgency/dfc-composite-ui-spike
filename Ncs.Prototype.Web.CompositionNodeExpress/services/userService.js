class userService {

    getCurrentUser(req) {
        return req.user || {};
    }

}

module.exports = userService;