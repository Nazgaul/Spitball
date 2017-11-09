<template>
    <div @keydown.right="currentIndex++" @keydown.left="currentIndex--"><close-action></close-action>
    <div class="white" v-if="(!currentCard||isEnded)">
        <h5 v-text="item.name"></h5>
        <span>Created by {{item.author}}</span>
        <p v-if="item.cards">{{item.cards.length}} Cards</p>
        <v-btn @click="$_startFlashcards">{{beginText}}</v-btn>
        <v-btn v-if="pinnCards" @click="$_startPinnsFlashcards">take {{pinnCards.length}} pinns cards</v-btn>
        <v-switch label="Shuffle cards"
                  v-model="shuffle"
                  color="light-green accent-3"></v-switch>
    </div>
    <div v-else>
        <input @keydown.right="currentIndex++" @keydown.left="currentIndex--">
        <v-btn @click="currentIndex--" :disabled="currentIndex==0">Back</v-btn>
        <v-btn @click="currentIndex++">Next</v-btn>
        The Original index:{{showList[currentIndex]}}
        Is Pin:{{isPinned}}
        <v-btn @click="$_pinCard">{{isPinned?'unpin':'pin'}} this card</v-btn>
        <vue-flashcard v-if="currentCard" :front="currentCard.front.text"
                       :imgFront="currentCard.front.image" :back="currentCard.cover.text" :imgBack="currentCard.cover.image"></vue-flashcard>
    </div>
    </div>
    
</template>
<script>
    import vueFlashcard from 'vue-flashcard';
    import closeAction from './itemActions.vue'
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
            isPinned() { return this.pinnCards.includes(this.showList[this.currentIndex].index)}
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

        components: { closeAction, vueFlashcard}
    }
</script>