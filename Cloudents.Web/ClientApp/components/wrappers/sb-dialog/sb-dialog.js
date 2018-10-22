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
        isFullScreen: {
            type: Boolean,
            default: false,
            required: false
        },
        fullWidth:{
            type: Boolean,
            default: false,
            required: false
        }

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
    },
    watch: {
        //changed from parent only!!!
        showDialog(){
            if(!!this.showDialog){
                this.show = true;
                console.log('width', this.fullWidth)
            }else{
                this.show = false;
            }
        },
        //changed locally
        show(){
            if(!this.show){
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