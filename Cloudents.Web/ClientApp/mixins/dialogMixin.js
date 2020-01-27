export default {
    methods: {
        closeDialog() {
            let { dialog, ...query } = this.$route.query;
            
            this.$router.replace({query});
        }
    }
}