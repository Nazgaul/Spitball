<template>
    <div class="select-university-container set-class">
            <div class="title-container">
                <div class="first-container">
                    <div><v-icon @click="lastStep()">sbf-arrow-back</v-icon></div>
                    <div><a class="next-container" @click="nextStep()">Done</a> </div>
                </div>
                <div class="select-class-string">
                    <span>Select Class</span> 
                </div>
                
            </div>
            <div class="explain-container">
                From {{schoolName}}
            </div>
            <div class="select-school-container">
                <v-combobox
                    v-model="selectedClasses"
                    :items="classes"
                    label="Type your Class name"
                    placeholder="Type your Class name"
                    clearable
                    solo
                    :search-input.sync="search"
                    :append-icon="''"
                    :clear-icon="'sbf-close'"
                    @click:clear="clearData()"
                    autofocus
                    multiple
                    chips
                >
                <template slot="no-data">
                    <v-list-tile v-if="showBox">
                        <div class="subheading">
                           <span>Create</span>
                            <v-chip>
                                {{ search }}
                            </v-chip>    
                        </div> 
                    </v-list-tile>
                    <!-- <v-list-tile>
                        <div class="subheading dark">Show all Classes</div>
                    </v-list-tile> -->
                </template>
                <template slot="selection" slot-scope="{ item, parent, selected }">
                   <v-chip class="chip-style" :class="{'dark-chip': !itemInList(item)}">
                       <span class="chip-button" @click="parent.selectItem(item)">
                           {{!!item.text ? item.text : item}} <v-icon class="chip-close">sbf-close</v-icon>
                       </span>
                    </v-chip> 
                </template>
                <template slot="item" slot-scope="{ index, item, parent }">
                    <v-list-tile-content>
                       <span v-html="$options.filters.boldText(item.text, search)">{{ item.text }}</span> 
                    </v-list-tile-content>
                </template>
                </v-combobox>
            </div>
        </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
export default {
    props:{
        fnMethods:{
            required:true,
            type:Object
        },
        enumSteps:{
            required:true,
            type:Object
        }
    },
    data(){
        return{
        classModel: '',
        search:'',
        }
        
    },
        watch:{
        search(val){
            if(val.length >= 3){
                this.updateClasses(val);
            }
        }
    },
    computed:{
        schoolName(){
            return this.getSchoolName()
        },
        showBox(){
            return this.search.length > 0;
        },
        classes(){
            return this.getClasses()
        },
        selectedClasses:{
            get(){
                this.classModel = this.getSelectedClasses() || this.classModel
                return this.classModel
            },
            set(val){
                this.classModel = val;
                this.updateSelectedClasses(val);
            }
        }
    },
    filters:{
        boldText(value, search){
           if (!value) return '';
            let match = value.toLowerCase().indexOf(search.toLowerCase()) > -1;
            if(match){
                let startIndex = value.toLowerCase().indexOf(search.toLowerCase())
                let endIndex = search.length;
                let word = value.substr(startIndex, endIndex)
                return value.replace(word, '<b>' + word + '</b>')
            }else{
                return value;
            }
        }
    },
    methods:{
        ...mapActions(['updateClasses', 'updateSelectedClasses', 'changeSelectUniState', 'assignClasses']),
        ...mapGetters(['getSchoolName', 'getClasses', 'getSelectedClasses']),
        clearData(){
            this.classModel = '';
            this.search = '';
            this.clearClasses();
        },
        lastStep(){
            this.fnMethods.changeStep(this.enumSteps.set_school);
        },
        nextStep(){
            console.log("kasdjfhasdlkf")
            //TODO add action update the server instead of 'updateSelectedClasses'
            this.assignClasses().then(()=>{
                this.fnMethods.changeStep(this.enumSteps.done);
            });
        },
        itemInList(item){
            if(typeof item !== 'object'){
                return false;
            }else{
                return true;
            }
        }
    }
}
</script>

<style lang="less" scoped>
    .chip-style{
        background-color: rgba(68, 82, 252, 0.09);
        &.dark-chip{
            background-color: rgba(68, 82, 252, 0.27);
        }
    }
    .chip-button{
        cursor: pointer;
        .sbf-close{
            font-size: 8px !important;
            margin-bottom: 3px;
            margin-left: 8px;
        }
    }
    .subheading{
        font-size: 16px;
        font-weight: normal;
        font-style: normal;
        font-stretch: normal;
        line-height: normal;
        letter-spacing: normal;
        color: rgba(0, 0, 0, 0.38);
        &.dark{
            font-size:13px!important;
            color: rgba(0, 0, 0, 0.54);
        }
    }
    
</style>
