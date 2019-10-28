export default {
    data() {
        return {
            submitted: false
        };
    },
    methods: {
        submitForm(val = true) {
            let retVal = !(val && this.submitted);
            this.submitted = val;
            return retVal;
        }
    }
}