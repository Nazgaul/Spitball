
<template>
    <v-dialog v-model="dialog" fullscreen content-class="dialog-choose" v-if="currentItem" class="settings" :overlay=false>
        <page-layout :type="currentType" :title="title" :search="!currentAction" :titleImage="(currentType==='course'&&!currentAction)?getUniversityImage:''" :isLoading="isLoading" :emptyText="emptyText" :items="items" :selectedCourse="selectedCourse" :closeFunction="$_closeButton">
            <button class="white--text" slot="extraClose" @click="$_closeButton" v-if="currentType!=='university'">DONE</button>
            <template slot="closeAction">
                <close-button v-if="currentType==='university'"></close-button>
                <i v-else class="sbf icon sbf-arrow-button"></i>
            </template>
            <template v-if="currentType==='course'" slot="courseExtraItem">
                <button class="add-course-btn ma-2" @click="showAddCourse=true" v-show="!showAddCourse">
                    <plus-button></plus-button>
                    <div>add course</div>
                </button>
                <div class="add-course-form ma-2 pa-2" v-show="showAddCourse">
                    <form @submit.prevent="$_submitAddCourse">
                        <v-text-field dark v-model="newCourseName" placeholder="Course Name"></v-text-field>
                        <v-btn @click="$_submitAddCourse">ADD</v-btn>
                    </form>
                </div>
            </template>

            <template slot="courseFirstTime" v-if="courseFirst&&showCourseFirst">
                <div class="first-time-message ma-3">
                    <div class="text">
                        <div>We are working hard on getting all the courses from your school into our system,</div>
                        <div><b>but we might have missed a few. If you can't find your course, use this icon to add it</b></div>
                    </div>
                    <div class="image">missing...</div>

                    <button @click="showCourseFirst=false">
                        <close-button ></close-button>
                    </button>
                </div>
            </template>

            <v-text-field light solo slot="inputField" @input="$_search" class="search-b" ref="searchText" :placeholder="currentItem.placeholder" prepend-icon="sbf-search"></v-text-field>

            <v-chip class="ma-2" slot="selectedItems" slot-scope="props" v-if="selectedCourse" label>
                <span class="name">{{ props.course.name }}</span>
                <button class="close pa-2" @click="$_removeCourse(props.course.id)">
                    <close-button></close-button>
                </button>
            </v-chip>
            <v-flex class="result" v-if="!currentAction" @click="$_clickItemCallback(keep)" slot-scope="props" slot="results">
                <component :is="'search-item-'+currentType" :item="props.item"></component>
            </v-flex>
            <component slot="actionContent" v-if="currentAction" :is="currentType+'-'+currentAction" @done="$_actionDone"></component>
        </page-layout>
    </v-dialog>
</template>
<script src="./searchItem.js">
</script>
<style src="./searchItem.less" lang="less"></style>
