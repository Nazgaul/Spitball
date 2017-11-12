
    import vueFlashcard from 'vue-flashcard';
    import closeAction from './itemActions.vue'
    import flashcardHeader from './flashcardHeader.vue'
    import flashcardContent from './flashcardContent.vue'
    import { mapActions } from 'vuex'
    export default {
        data() {
            return {
                item: {},
                shuffle: false,
                currentIndex: -1,
                showList: [],
                pinnCards:[]
            }
        },
        computed: {
            beginText() { return this.isEnded?'Start Over':'Start'},
            isEnded() {
                let retVal = this.currentIndex == this.showList.length;
                if (retVal) this.showList = this.item.cards.map((item, index) => { return { index, data: item } })
                return retVal
            },
            currentCard() { if (this.currentIndex > -1 && this.showList && this.currentIndex < this.showList.length) return this.showList[this.currentIndex].data},
            isPinned() { return this.pinnCards.includes(this.showList[this.currentIndex].index) },
            showCards() { return (this.currentCard&&!this.isEnded)}
        },
        created() {
            this.getPreview({ type: 'flashcard', id: this.id }).then(res => {
                console.log('falshcard' + res)
                this.item = res
                this.showList = this.item.cards.map((item, index) => { return { index, data: item } })
                this.pinnCards = this.item.pins;
            })
            let vm = this;
            window.addEventListener('keyup', function (event) {
                if (event.keyCode == 37 && vm.currentIndex > 0 && !vm.isEnded) vm.currentIndex--;
                else if (vm.currentIndex >= 0 && event.keyCode == 39 && !vm.isEnded) vm.currentIndex++;
            });
        },
        methods: {
            ...mapActions(['getPreview']),
            $_startFlashcards() {
                this.currentIndex = 0;
                let list = this.showList;
                this.showList = this.shuffle ? list.sort(() => Math.random() - 0.5) : list.sort((a,b)=> a.index - b.index)
            },
            $_startPinnsFlashcards() {
                this.showList = this.showList.filter((i) => this.pinnCards.includes(i.index))
                this.$_startFlashcards();
            },
            $_pinCard() {
                let indexToCheck = this.showList[this.currentIndex].index;
                console.log(indexToCheck);
                this.isPinned ? this.pinnCards = this.pinnCards.filter((i) => i != indexToCheck) : this.pinnCards.push(indexToCheck)  
                //TODO add and remove from db
            }
        },
        props: { id: { type: String } },

        components: { closeAction, vueFlashcard, flashcardHeader, flashcardContent}
    }
