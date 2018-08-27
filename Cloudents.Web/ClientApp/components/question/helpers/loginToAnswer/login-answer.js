import {mapGetters, mapActions} from 'vuex'

export default {
    props: {
    },
    data: function () {
        return {}
    },
    computed: {
        ...mapGetters({
            loginDialogState: 'loginDialogState'
        }),
    },
    methods: {
        ...mapActions(["updateLoginDialogState"]),
        //close dialog
        requestDialogClose() {
            this.updateLoginDialogState(false)
        },
    },
    beforeDestroy(){
        this.updateLoginDialogState(false)
    }
}