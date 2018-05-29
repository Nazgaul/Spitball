import UniSearchInput from '../../helpers/uniSearchInput.vue';

export default {
    components: {UniSearchInput},
    data() {
        return {
            test:'',
            username: 'Rocket Rocket',
            university:''
        }
    },
    methods: {
        submitQuestion() {
            var self = this;
            questionService.postQuestion(this.subject.id, this.textAreaValue, this.price, this.files)
                .then(function () {
                    self.$router.push({path: '/ask', query: {q: ''}});
                });
        },
        addFile(filename){
            this.files.push(filename);
        },
    },
    computed: {
        validForm() {
            return this.subject && this.textAreaValue.length && this.price >= 0.5;
        }
    },
    created() {
        var self = this;
        // questionService.getSubjects().then(function (response) {
        //     self.subjectList = response.data
        // })
    }
}