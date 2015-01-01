angular.module('ajax').factory('account',
    ['ajaxService',
    function (ajaxService) {
    	function buildPath(path) {
    		return '/account/' + path + '/';
    	}
    	return {
    		details: function (data) {
    			return ajaxService.get(buildPath('userDetails'), data);
    		},
    		changeLanguage: function (data) {
    			return ajaxService.post(buildPath('changelanguage'), data);
    		},
    		changeLocale: function (data) {
    			return ajaxService.post(buildPath('changelocale'), data);
    		},
    		//submitCode: function (data) {
    		//	return ajaxService.post(buildPath('entercode'), data);
    		//},
    		facebookLogin: function (data) {
    			return ajaxService.post(buildPath('facebooklogin'), data);
    		},
    		login: function (data) {
    			return ajaxService.post(buildPath('login'), data);
    		},
    		register: function (data) {
    			return ajaxService.post(buildPath('register'), data);
    		}//,
    		//disableFirstTime: function (data) {
    		//	return ajaxService.post('/account/firsttime/', data);
    		//}
    	};


    }]
);