(function () {
    angular.module('app').controller('contactUsController', contactUs);
    contactUs.$inject = ['ajaxService', '$scope'];
    function contactUs(ajaxService, $scope) {
        var cu = this;
        var map = new GMaps({
            div: '#map',
            lat: 31.911377,
            lng: 34.809119
        });
        var marker = map.addMarker({
            lat: 31.911377,
            lng: 34.809119,
            title: 'Cloudents, Inc.',
            infoWindow: {
                content: "<b>Cloudents, Inc.</b> Bergman 2 <br>Rehovot, IL"
            }
        });

        marker.infoWindow.open(map, marker);
        cu.feedback = feedback;


        function feedback(form) {
            $scope.app.showToaster('thank you', 'feedback');
            ajaxService.post('/home/feedback/', {
                subject: cu.subject,
                email: cu.email,
                name: cu.name,
                message: cu.feedbackTxt
            });
            cu.subject = cu.email = cu.name = cu.feedbackTxt = '';
            $scope.app.resetForm(form);
        }
    }
})()