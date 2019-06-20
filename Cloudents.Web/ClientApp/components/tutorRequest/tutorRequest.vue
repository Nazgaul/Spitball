<template>
  <transition name="fade">
    <v-form class="request-tutor-container" v-model="validRequestTutorForm" ref="tutorRequestForm">
      <v-card>
        <v-toolbar class="headline" height="45" dark color="#1B2441">
          <v-icon class="header-icon title">sbf-person-icon</v-icon>
          <v-toolbar-title class="subheading" v-language:inner="isTutorList? 'tutorRequest_title_tutor_list': 'tutorRequest_title'"></v-toolbar-title>
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-card-text>
          <v-container grid-list-md>
            <v-layout wrap>
              <v-flex xs12 md4 v-if="!isAuthUser">
                  <v-text-field 
                    v-model="guestName"
                    :rules="[rules.required]"
                    :placeholder="guestNamePlaceHolder"
                    autocomplete="off"/>
              </v-flex>
              <v-flex xs12 md4 v-if="!isAuthUser">
                  <v-text-field 
                    v-model="guestMail"
                    type="email"
                    :rules="[rules.required, rules.email]"
                    :placeholder="guestEmailPlaceHolder"
                    autocomplete="off"/>
              </v-flex>      
              <v-flex xs12 md4 v-if="!isAuthUser">
                  <v-text-field 
                     
                    mask="phone"
                    v-model="guestPhone"
                    :rules="[rules.required]"
                    :placeholder="guestPhoneNumberPlaceHolder"
                    autocomplete="off"/>
              </v-flex>

              <v-flex xs12 v-if="!isAuthUser">
                  <v-textarea  
                    rows="3"
                    name="add-request-textarea"
                    :label="topicPlaceholder"
                    :rules="[rules.required, rules.maximumChars]"
                    v-model="tutorRequestText"/>
            </v-flex>
            <v-flex xs12 v-else>
                  <v-textarea 
                    rows="2"
                    name="add-request-textarea"
                    :label="topicPlaceholder"
                    :rules="[rules.required, rules.maximumChars]"
                    v-model="tutorRequestText"/>
            </v-flex>

              <v-flex xs12 md6 v-if="!isAuthUser" >
                  <v-autocomplete
                    @keyup="searchCourses"
                    flat
                    hide-no-data
                    :append-icon="''"
                    :menu-props="{contentClass:'courses-select-list'}"
                    v-model="tutorCourse"
                    :items="suggestsCourses"
                    :placeholder="coursePlaceholder"
                    :rules="[rules.required]"                  
                  ></v-autocomplete>
              </v-flex>
              <v-flex xs12 v-else>
                  <v-autocomplete
                    @keyup="searchCourses"
                    flat
                    hide-no-data
                    :append-icon="''"
                    :menu-props="{contentClass:'courses-select-list'}"
                    v-model="tutorCourse"
                    :items="suggestsCourses"
                    :placeholder="coursePlaceholder"
                    :rules="[rules.required]"  
                  ></v-autocomplete>
              </v-flex>
              
              <v-flex xs12 md6 v-if="!isAuthUser">
                  <v-autocomplete
                    @keyup="searchUniversities"
                    flat
                    hide-no-data
                    :append-icon="''"
                    :menu-props="{contentClass:'courses-select-list'}"
                    v-model="guestUniversity"
                    :items="suggestsUniversities"
                    :placeholder="universityPlaceHolder"
                    return-object
                    no-filter
                  ></v-autocomplete>
              </v-flex>

            </v-layout>
          </v-container>
        </v-card-text>
        <v-card-actions class="alignEnd">
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" :disabled="btnRequestLoading" flat @click="tutorRequestDialogClose()">{{btnClosePlaceholder}}</v-btn>
          <v-btn color="#4452fc" :loading="btnRequestLoading" round depressed dark @click="sendRequest()">{{btnSubmitPlaceholder}}</v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </transition>
</template>


<script src="./tutorRequest.js"></script>

<style lang="less" src="./tutorRequest.less"></style>