
    import closeAction from './itemActions.vue'
    import flashcardHeader from './flashcardHeader.vue'
    import flashcardContent from './flashcardContent.vue'
    import { mapActions, mapGetters } from 'vuex'
    export default {
        data() {
            return {
                item: {},
                shuffle: false,
                currentIndex: -1,
                showList: [],
                slideFront:true
            };
        },
        computed: {
            ...mapGetters(['pinnedCards']),
            beginText() { return this.isEnded?'Start Over':'Start'},
            isEnded() {
                let retVal = this.currentIndex === this.showList.length;
                if (retVal) {
                    this.showList = this.item.cards.map((item, index) => ({ index, data: item }));
                    this.slideFront = true;
                }
                return retVal;
            },
            currentPinn() { return new Set(this.pinnedCards[this.$attrs.id])},
            currentCard() { if (this.currentIndex > -1 && this.showList && this.currentIndex < this.showList.length) return this.showList[this.currentIndex].data},
            showCards() { return (this.currentCard&&!this.isEnded)}
        },
        created() {
            // this.getPreview({ type: 'flashcard', id: this.id }).then(({data:body}) => {
            this.getPreview({ type: 'flashcard', id: this.id }).then((body) => {
                this.item = body;
                this.showList = this.item.cards.map((item, index) => ({ index, data: item } ))
            });
            window.addEventListener('keyup', this.handleArrow);
        },
        beforeDestroy(){
            window.removeEventListener('keyup',this.handleArrow);
        },
        methods: {
            ...mapActions(['getPreview']),
            $_startFlashcards() {
                this.currentIndex = 0;
                let list = this.showList;
                this.showList = this.shuffle ? list.sort(() => Math.random() - 0.5) : list.sort((a,b)=> a.index - b.index);
            },
            $_startPinnsFlashcards() {
                this.showList = this.showList.filter((i) => this.currentPinn.has(i.index));
                this.$_startFlashcards();
            },

            handleArrow (event){
                if (event.keyCode === 37 && this.currentIndex > 0 && !this.isEnded) this.currentIndex--;
                else if (this.currentIndex >= 0 && event.keyCode === 39 && !this.isEnded) this.currentIndex++;
            }
        },
<<<<<<< Updated upstream

        components: {
            closeAction,
            flashcardHeader,
            flashcardContent,
            shuffleIcon: () => import("./svg/shuffle-icon.svg"),
            pinIcon: () => import("./svg/pin-icon.svg")
            }
=======
        props:{id:{Number}},
        components: { closeAction, flashcardHeader, flashcardContent}
>>>>>>> Stashed changes
    }
