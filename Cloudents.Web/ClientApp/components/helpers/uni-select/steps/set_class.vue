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
                >
                <template slot="no-data">
                    <v-list-tile v-show="showBox">
                        <div class="subheading">Keep typing we are searching for you</div> 
                    </v-list-tile>
                    <v-list-tile>
                        <div class="subheading dark">Show all Classes</div>
                    </v-list-tile>
                </template>
                <template slot="selection" slot-scope="{ item, parent, selected }">
                   <span style="font-weight:bold;">{{!!item.text ? item.text : item}}</span> 
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
        classes: [],
        search:''
        }
        
    },
    computed:{
        ...mapGetters(["getSchoolName"]),
        schoolName(){
            return this.getSchoolName
        },
        showBox(){
            return this.search.length > 0;
        },
    },
    methods:{
        clearData(){
            console.log("data cleared")
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
