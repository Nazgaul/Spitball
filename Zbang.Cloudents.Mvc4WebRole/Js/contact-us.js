var ContactUs = function () {

    return {
        //main function to initiate the module
        init: function () {
			var map;
			$(document).ready(function(){
			  map = new GMaps({
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
			});
        }
    };

}();

$(function () {
    ContactUs.init();
});