
export default {
    props: {
        showDialog: {
            type: Boolean,
            default: false
        },

        popUpType: {
            type: String,
            required: true
        },
        contentClass: {
            type: String,
            required: false
        },
        fullWidth: {
            type: Boolean,
            required: false,
            default: false
        },
        activateOverlay: {
            type: Boolean,
            required: false,
            default: false
        },
        onclosefn: {
            required: false
        },
        isPersistent: false,
        transitionAnimation: '',
        maxWidth: {
            type: String,
            default: '720px',
            required: false
        },

    },
    data: function () {
        return {
            show: false
        };
    },
    computed: {
        OverlayActive() {
            if (this.$vuetify.breakpoint.xs) {
                return !this.activateOverlay;
            } else {
                return false;
            }
        }
    },
    watch: {
        //changed from parent only!!!
        showDialog() {
            if (!!this.showDialog) {
                if (this.$vuetify.breakpoint.xs) {
                    document.body.classList.add('noscroll')
                }
                this.show = true;
            } else {
                document.body.classList.remove('noscroll')
                this.show = false;
            }
        },
        //changed locally
        show() {
            if (!this.show) {
                if (!!this.onclosefn) {
                    this.onclosefn();
                }
            }
        }
    },
}