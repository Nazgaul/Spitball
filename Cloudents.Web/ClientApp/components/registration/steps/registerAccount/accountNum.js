export default {
    computed: {
        accountNum: function () {
            return 'accountNum123456'
        }
    },
    methods: {
        copyToClipboard() {
            let el = this.$el.querySelector("#input-url");

            // handle iOS as a special case
            if (navigator.userAgent.match(/ipad|ipod|iphone/i)) {
                // convert to editable with readonly to stop iOS keyboard opening
                el.contentEditable = true;
                el.readOnly = true;

                // create a selectable range
                var range = document.createRange();
                range.selectNodeContents(el);

                // select the range
                var selection = window.getSelection();
                selection.removeAllRanges();
                selection.addRange(range);
                el.setSelectionRange(0, 999999);
            }
            else {
                el.select();
            }

            // execute copy command
            document.execCommand('copy');
            el.blur();
        },

    }
}