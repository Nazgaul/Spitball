<template>
        <div class="select-university-container mini" :class="{'selected': (!!search && search.length > 10) || !!university}">
            <div class="title-container">
                Select School
                <a v-show="search.length > 10 || !!university" class="next-container" @click="nextStep()">Next</a>  
            </div>
            <div class="select-school-container">
                <v-combobox
                    v-model="university"
                    :items="universities"
                    label="Type your school name"
                    placeholder="Type your school name"
                    clearable
                    solo
                    :search-input.sync="search"
                    :append-icon="''"
                    :clear-icon="'sbf-close'"
                    @click:clear="clearData()"
                    autofocus
                >
                <template slot="no-data">
                    <v-list-tile v-show="showBox">
                        <div class="subheading">Keep typing we are searching for you</div> 
                    </v-list-tile>
                    <v-list-tile>
                        <div class="subheading dark">Show all Schools</div>
                    </v-list-tile>
                </template>
                <template slot="selection" slot-scope="{ item, parent, selected }">
                   <span style="font-weight:bold;">{{!!item.text ? item.text : item}}</span> 
                </template> 
                 <!-- <template slot="item" slot-scope="{ index, item, parent }">
                        <v-list-tile-content>
                            {{item.text}}
                        </v-list-tile-content>
                    </template> -->
                </v-combobox>
                <!-- <input type="text" placeholder="Type your school name" autofocus>-->
                <!-- <v-icon>sbf-arrow-right</v-icon> -->
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
            university: '',
            search: ''
        }
    },
    watch:{
        search(val){
            if(val.length > 3){
                let uniArr = [];
                uniArr.push({text:'the collage of new jersey'})
                uniArr.push({text:'new jersey city'})
                uniArr.push({text:'new mexico state uni'})
                this.updateUniversities(uniArr);
            }
        }
    },
    methods:{
        ...mapActions(["updateUniversities", "clearUniversityList"]),
        clearData(){
            this.search = null;
            this.university = undefined;
            this.clearUniversityList();
        },
        nextStep(){
            console.log("nextStep")
            console.log("search: " + this.search)
            console.log("university: " + this.university)
        }
        
    },
    computed:{
        ...mapGetters(["getUniversities"]),
        showBox(){
            return this.search.length > 0;
        },
        universities(){
            return this.getUniversities;
        }
    }
}
</script>

<style lang="less">
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
