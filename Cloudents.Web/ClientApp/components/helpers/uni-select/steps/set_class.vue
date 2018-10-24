<template>
    <div class="select-university-container meduim">
            <div class="title-container">
                <div class="first-container">
                    <v-icon @click="lastStep()">sbf-arrow-back</v-icon>
                    <a class="next-container" @click="nextStep()">Done</a>  
                </div>
                <div>
                    <span>Select Class</span> 
                </div>
                
            </div>
            <div class="explain-container">
                From {{schoolName}}
            </div>
            <div class="select-school-container">
                <v-combobox
                    v-model="classModel"
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
                    deletable-chips
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
                <!-- <template slot="selection" slot-scope="{ item, parent, selected }">
                   <span style="font-weight:bold;">{{!!item.text ? item.text : item}}</span> 
                </template> -->
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
        search:''
        }
        
    },
        watch:{
        search(val){
            if(val.length > 3){
                let classArr = [];
                classArr.push({text:'biology'})
                classArr.push({text:'chemistry'})
                classArr.push({text:'lolr'})
                classArr.push({text:'blahda'})
                this.updateClasses(classArr);
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
        }
    },
    filters:{
        boldText(value, search){
            //mark the text bold according to the search value
            if (!value) return '';
            return value.replace(search, '<b>' + search + '</b>')
        }
    },
    methods:{
        ...mapActions(['updateClasses']),
        ...mapGetters(['getSchoolName', 'getClasses']),
        clearData(){
            this.classModel = '';
            this.search = '';
            this.clearClasses();
        },
        lastStep(){
            this.fnMethods.changeStep(this.enumSteps.set_school);
        },
        nextStep(){
            console.log("next Step")
        }
    }
}
</script>

<style lang="less" scoped>
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
