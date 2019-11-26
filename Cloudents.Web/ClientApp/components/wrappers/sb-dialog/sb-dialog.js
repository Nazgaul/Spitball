import { mapGetters, mapActions } from 'vuex';

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
        ...mapGetters({
            getLoginDialog: 'getLoginDialog',
        }),
        OverlayActive() {
            if (this.$vuetify.breakpoint.xs) {
                if (this.activateOverlay) {
                    return false;
                } else {
                    return true;
                }
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
                    document.getElementsByTagName("body")[0].className = "noscroll";
                }
                this.show = true;
            } else {
                document.body.removeAttribute("class", "noscroll");
                this.show = false;
            }
        },
        //changed locally
        show() {
            if (!this.show) {
                if (!!this.onclosefn) {
                    this.onclosefn();
                }
                this.updateLoginDialog(false);
                this.updateNewQuestionDialogState(false);
                // this.$root.$emit('closePopUp', this.popUpType);
            }
        }
    },

    methods: {
        ...mapActions(['updateLoginDialog', 'updateNewQuestionDialogState']),
    },


}