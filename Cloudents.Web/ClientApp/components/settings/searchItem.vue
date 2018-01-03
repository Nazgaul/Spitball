
<template>
    <page-layout v-if="currentType" :type="currentType" :title="title" :search="!currentAction" :showAdd="showAdd" :titleImage="(currentType==='course'&&!currentAction)?getUniversityImage:''" :isLoading="isLoading" :items="items" :selectedCourse="selectedCourse" :closeFunction="$_closeButton">
            <button class="done-btn" slot="courseExtraClose" @click="$emit('input',false)">DONE</button>
        <template slot="closeAction">
            <i v-if="isFirst&&currentType==='course'" class="sbf icon sbf-arrow-button"></i>
            <i v-else class="sbf icon sbf-close"></i>
        </template>
        
        <template v-if="showCreateCourse" slot="courseExtraItem">
            <div class="add-course-form mx-2 mt-4 py-3 px-3" v-if="!isMobile">
                <form @submit.prevent="$_submitAddCourse">
                    <div class="form-title">Still don't see your class?</div>
                        <v-text-field light v-model="newCourseName" placeholder="Type it in here:"></v-text-field>
                    <div class="actions">
                        <v-btn class="save" :disabled="!newCourseName" @click="$_submitAddCourse">save</v-btn>
                        <v-btn class="clear" :disabled="!newCourseName" @click="$_clearAddCourse">clear</v-btn>
                    </div>
                </form>
            </div>
            <div v-else @click.prevent="showAdd=true" class="mx-2 mt-1 py-3 px-3">
                <v-text-field light v-model="newCourseName" placeholder="Type it in here:" class="input-group--focused" label="Still don't see your class?"></v-text-field>
        </div>
        </template>
        <template v-if="showCreateCourse&&isMobile" slot="courseMobileExtraItem">
            <div class="add-course-form mx-2 mt-4 py-3 px-3">
                <form @submit.prevent="$_submitAddCourse">
                    <!--<div class="form-title">Still don't see your class?</div>-->
                    <v-text-field light v-model="newCourseName" label="Type it in here:" ></v-text-field>
                    <div class="actions">
                        <v-btn class="save" :disabled="!newCourseName" @click="$_submitAddCourse">save</v-btn>
                        <v-btn class="clear"  @click="$_clearAddCourse">Cancel</v-btn>
                    </div>
                </form>
            </div>
        </template>


        <v-text-field light solo slot="inputField" @input="(val)=>{val.length > 2 ? isLoading = true : this.isLoading = false}" v-model.lazy="val" class="search-b" ref="searchText" autofocus :placeholder="currentItem.placeholder" prepend-icon="sbf-search"></v-text-field>
        <template  slot="selectedItems" slot-scope="props" v-if="selectedCourse">
            <v-chip v-if="!isMobile" label class="ma-2">
                <span class="name" :title="props.course.name">{{ props.course.name }}</span>
                <button class="close pa-2" @click="$_removeCourse(props.course.id)">
                    <i class="sbf icon sbf-close"></i>
                </button>
            </v-chip>
            <template v-else-if="!val">
                <component :is="'search-item-'+currentType" :item="props.course" :isChecked="true"></component>
            </template>
        </template>
            <v-flex class="result" v-if="!currentAction" @click="$_clickItemCallback(keep)" slot-scope="props" slot="results">
            
            <component :is="'search-item-'+currentType" :item="props.item"></component>
        </v-flex>
        <component slot="actionContent" v-if="currentAction" :is="currentType+'-'+currentAction" @done="$_actionDone"></component>
        <template slot="universityEmptyState" v-if="noResults">
            <div class="uni-empty-state">
                <div>Can't find your school?</div>
                <div> email us at <a  onclick="Intercom('showNewMessage')">support@spitball.co</a> and we will add it.</div>
            </div>
        </template>
    </page-layout>
</template>
<script src="./searchItem.js">
</script>
<style src="./searchItem.less" lang="less"></style>
