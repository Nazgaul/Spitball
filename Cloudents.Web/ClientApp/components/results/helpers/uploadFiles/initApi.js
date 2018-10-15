let clientId = '1063816156150-vilh4o94nl0uld8lg466f5kj2qgduif2.apps.googleusercontent.com',
    //appId = 'AIzaSyC1HN_GRKd7_2lWfjXBhM0ppQobiZxgy_M',
    appId = 'rLic0emOA2iZ1Poh8L-326rW',
    scope = ['https://www.googleapis.com/auth/drive.readonly'],
    access_token, contacts = [], clientLoaded = false, clientLoading = false, pickerApiLoaded = false;
var oauthToken;
var callbackFunction = null;

   const googlePickerApi = {
       // Use the Google API Loader script to load the google.picker script.
       loadPicker: function (callback) {
           let self = this;
           callbackFunction = callback;
           console.log('gapi loader from!!!', gapi);
           gapi.load('auth', {'callback': self.onAuthApiLoad});
           gapi.load('picker', {'callback': self.onPickerApiLoad});
       },

       onAuthApiLoad: function () {
           window.gapi.auth.authorize(
               {
                   'client_id': clientId,
                   'scope': scope,
                   'immediate': false
               },
               function (authResult) {
                   if (authResult && !authResult.error) {
                       oauthToken = authResult.access_token;
                       googlePickerApi.createPicker();
                   }
               },);
       },

       onPickerApiLoad: function () {
           pickerApiLoaded = true;
           googlePickerApi.createPicker();
       },


       // Create and render a Picker object for searching images.
       createPicker: function () {
           if (pickerApiLoaded && oauthToken) {
               var view = new google.picker.View(google.picker.ViewId.DOCS);
               view.setMimeTypes("image/png,image/jpeg,image/jpg");
               var picker = new google.picker.PickerBuilder()
                   .setTitle('Please select documents for upload')
                   .setOrigin(window.location.protocol + '//' + window.location.host)
                  // .enableFeature(google.picker.Feature.NAV_HIDDEN)
                   .enableFeature(google.picker.Feature.MULTISELECT_ENABLED)
                   .setAppId(appId)
                   .setOAuthToken(oauthToken)
                   .addView(view)
                   .addView(new google.picker.DocsUploadView())
                   //.setDeveloperKey(developerKey)
                   .setCallback(callbackFunction)
                   .build();
               picker.setVisible(true);
           }
       },

       // // A simple callback implementation.
       // pickerCallback: function (data) {
       //     if (data.action == google.picker.Action.PICKED) {
       //         let fileId = data.docs[0].id;
       //         console.log('docs', data);
       //     }
       // }
   };
       export default googlePickerApi
