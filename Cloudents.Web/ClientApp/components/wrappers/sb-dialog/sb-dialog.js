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