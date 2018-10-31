import {mapGetters, mapActions} from 'vuex';
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
            default:false
        },
        activateOverlay:{
            type: Boolean,
            required: false,
            default: false
        },
        onclosefn: {
            required: false
        },
        isPersistent: false,
        transitionAnimation: ''
    },
    data: function () {
        return {
            show: false
        }
    },
    computed: {
        ...mapGetters({
            loginDialogState: 'loginDialogState',
        }),
        OverlayActive(){
            if(this.$vuetify.breakpoint.xs){
                if(this.activateOverlay){
                    return false
                }else{
                    return true;
                }
            }else{
                return false;
            }
        }
    },
    watch: {
        //changed from parent only!!!
        showDialog(){
            if(!!this.showDialog){
                this.show = true;
            }else{
                this.show = false;
            }
        },
        //changed locally
        show(){
            if(!this.show){
                if(!!this.onclosefn){
                    this.onclosefn();
                }
                this.updateLoginDialogState(false);
                this.updateNewQuestionDialogState(false);
                this.$root.$emit('closePopUp', this.popUpType);
            }
        }
    },

    methods: {
        ...mapActions(['updateLoginDialogState', 'updateNewQuestionDialogState']),
    },


}