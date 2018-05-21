import questionCard from "./../../question/helpers/question-card/question-card.vue";
import marketItem from "./../helpers/market-item/market-item.vue"

export default {
    components: {questionCard, marketItem},
    data() {
        return {
            marketItem:[
                {image:'https://upload.wikimedia.org/wikipedia/commons/thumb/8/8f/Bachalpsee_reflection.jpg/300px-Bachalpsee_reflection.jpg'},
                {image:'https://upload.wikimedia.org/wikipedia/commons/thumb/8/8f/Bachalpsee_reflection.jpg/300px-Bachalpsee_reflection.jpg'},
                {image:'https://upload.wikimedia.org/wikipedia/commons/thumb/8/8f/Bachalpsee_reflection.jpg/300px-Bachalpsee_reflection.jpg'},
                {image:'https://upload.wikimedia.org/wikipedia/commons/thumb/8/8f/Bachalpsee_reflection.jpg/300px-Bachalpsee_reflection.jpg'}
            ]
        }
    },
    methods:{        
    },
    computed:{        
        isMobile(){return this.$vuetify.breakpoint.xsOnly;},    
        validForm() {
            return this.textAreaValue.length
        }
    }
}