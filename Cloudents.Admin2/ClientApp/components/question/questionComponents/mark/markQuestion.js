import { getAllQuesitons, acceptAnswer } from './markQuestionService'

export default {
    data(){
        return {
            questions:[],
            page: 0,
            scrollLock: false
        }
    },
    methods:{
            openQuestion(url){
                window.open(url, "_blank");
            },
            acceptQuestion(question, answer){
                acceptAnswer(question.toServer(answer.id)).then(()=>{
                    //alert(`SUCCESS: question id: ${question.id} accepted answer id: ${answer.id}`)
                    //remove the question from the list
                    this.$toaster.success(`Question id: ${question.id} accepted answer id: ${answer.id}`)
                    let questionIndex = this.questions.indexOf(question)
                    this.questions.splice(questionIndex, 1);
                }, ()=>{
                    this.$toaster.error(`ERROR FAILED TO ACCEPT question id: ${question.id} answer id: ${answer.id}`)
                })
            },
            advancePage(){
                this.page++
            },
            getQuestions(){
                getAllQuesitons(this.page).then((questionsResponse) => {
                    questionsResponse.forEach(question=>{
                        this.questions.push(question);
                    })
                    this.advancePage();
                    this.scrollLock = false;
                })
            },
            handleScroll(event){
                let offset = 2000;
                if(event.target.scrollHeight - offset < event.target.scrollTop){
                    if(!this.scrollLock){
                        this.scrollLock = true;
                        this.getQuestions();
                    }
                }
            }
    },
    created(){
        this.getQuestions();
    },
    beforeMount () {
        this.$nextTick(function(){
            let containerElm = document.getElementById('app-body');
            containerElm.addEventListener('scroll', this.handleScroll);
        })
      },
    beforeDestroy () {
      let containerElm = document.getElementById('app-body');
      containerElm.removeEventListener('scroll', this.handleScroll);
    }
}