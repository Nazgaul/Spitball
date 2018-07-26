export default {
    props: {

    },
    data: function () {
        return {}
    },
    watch: {},
    methods: {
        //close dialog
        requestDialogClose() {
            this.$root.$emit('closePopUp', 'loginPop')
        },
    },
}