(function () {
    'use strict';
    angular.module('app.box.feed', ['app.ajaxservice']);
})();
(function () {
    'use strict';
    angular.module('app.box.quizzes', ['app.ajaxservice']);
})();
(function () {
    'use strict';
    angular.module('app.box.items', ['app.ajaxservice']);
})();

(function () {
    'use strict';
    angular.module('app.box.members', ['app.ajaxservice']);
})();
(function () {
    'use strict';
    angular.module('app.box', ['app.ajaxservice',  'app.box.feed', 'app.box.quizzes', 'app.box.items', 'app.box.members']);
})();

