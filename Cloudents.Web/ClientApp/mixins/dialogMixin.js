export default {
    methods: {
        closeDialog() {
            let { dialog, ...query } = this.$route.query;
            console.log(dialog); // need to keep for destructure query object for eslint error
            this.$router.replace({query});
        }
    }
}