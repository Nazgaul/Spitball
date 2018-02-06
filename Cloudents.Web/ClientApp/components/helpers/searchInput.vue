<template>
    <v-menu :allow-overflow="true" offset-y full-width content-class="search-menu" :value="menuOpen">
                            <span slot="activator">
                                <v-text-field type="search" solo
                                              @keyup.enter="search" autocomplete="off"
                                              required name="q"
                                              :class="{'record':isRecording}"
                                              id="transcript"
                                              v-model.trim="msg" :placeholder="placeholder"
                                              prepend-icon="sbf-search" :append-icon="voiceAppend"
                                              :append-icon-cb="$_voiceDetection"></v-text-field>
                            </span>
        <v-list>
            <v-subheader>Some things you can ask me:</v-subheader>
            <template v-for="(item, index) in suggestList">
                {{item.type}}
                <v-list-tile @click="selectos({item:item.text,index})" :key="index">
                    <v-list-tile-action hidden-xs-only>
                        <v-icon>sbf-search</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title v-text="item.text"></v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>
            </template>
        </v-list>
    </v-menu>
</template>

<script>
    import { micMixin } from './mic';

    import {mapGetters} from 'vuex'
    const MAX_SUGGEST_NUM=10;
    const homeSuggest = [
        "Flashcards for financial accounting",
        "Class notes for my Calculus class",
        "When did World War 2 end?",
        "Difference between Meiosis and Mitosis",
        "Tutor for Linear Algebra",
        "Job in marketing in NYC",
        "The textbook - Accounting: Tools for Decision Making",
        "Where can I get a burger near campus?"
    ];
    const SUGGEST_TYPE={history:"history",buildIn:"buildIn"};
    export default {
        name: "search-input",
        mixins:[micMixin],
        props:{menuOpen:{type:Boolean,default:true},
            placeholder:{type:String},
            userText:{String},
            submitRoute:{String}},
        computed:{
            ...mapGetters({'globalTerm':'currentText'}),
            ...mapGetters(['historyTermSet']),
            suggestList(){
                let historyList=[...this.historyTermSet];
                let set=[...new Set([...historyList.reverse(),...homeSuggest])].filter(i=>i.toLowerCase().includes(this.msg.toLowerCase()));
                return set.slice(0,MAX_SUGGEST_NUM-1).map(i=>({text:i,type:(this.historyTermSet.includes(i)?SUGGEST_TYPE.history:SUGGEST_TYPE.buildIn)}));
            },
            isHome(){return this.$route.name==='home'}
        },
        watch: {
            userText(val) {
                this.msg = val;
            }
        },
        methods:{
            selectos({item,index}) {
                this.msg = item;
                this.$ga.event('Search','Suggest', `#${index+1}_${item}`);
                this.search();
            },
            search() {
                if(this.submitRoute){
                    this.$router.push({path:this.submitRoute,query:{q:this.msg}});
                }
                else if (this.msg) {
                    this.$router.push({ name: "result", query: { q: this.msg } });
                }
                // to remove keyboard on mobile
                this.$nextTick(() => {
                    this.$el.querySelector('input').blur();
                    this.$el.querySelector('form').blur();
                });
            },
            //callback for mobile submit mic
            submitMic(){
                this.search();
            }
        },
        created(){
            if(!this.isHome) {
                this.msg = this.userText ? this.userText : this.globalTerm;
            }
        }
    }
</script>
<style lang="less" src="./searchInput.less"></style>