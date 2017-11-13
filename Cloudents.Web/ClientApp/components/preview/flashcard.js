
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
            }
        },
        computed: {
            ...mapGetters(['pinnedCards']),
            beginText() { return this.isEnded?'Start Over':'Start'},
            isEnded() {
                let retVal = this.currentIndex == this.showList.length;
                if (retVal) {
                    this.showList = this.item.cards.map((item, index) => ({ index, data: item }))
                    this.slideFront = true;
                }
                return retVal
            },
            currentPinn() { return new Set(this.pinnedCards[this.$attrs.id])},
            currentCard() { if (this.currentIndex > -1 && this.showList && this.currentIndex < this.showList.length) return this.showList[this.currentIndex].data},
            showCards() { return (this.currentCard&&!this.isEnded)}
        },
        created() {
            this.getPreview({ type: 'flashcard', id: this.id }).then(res => {
                this.item = res
                this.showList = this.item.cards.map((item, index) => ({ index, data: item } ))
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
                this.showList = this.showList.filter((i) => this.currentPinn.has(i.index))
                this.$_startFlashcards();
            }
        },

        components: { closeAction, flashcardHeader, flashcardContent}
    }
