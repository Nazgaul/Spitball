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
        }
    },
    data: function () {
        return {
            show: false
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
                this.$root.$emit('closePopUp', this.popUpType)
            }
        }
    },
    computed:{
    },
    methods: {
    },
}