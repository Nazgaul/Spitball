export default {
    data() {
        return {
            submitted: false
    }
    },
    methods:{
        submitForm(val=true){this.submitted=val;}
    }
}