<template>
  <transition name="fade">
    <v-form class="request-tutor-container" v-model="validRequestTutorForm" ref="tutorRequestForm">
      <v-card>
        <v-toolbar class="headline" height="45" dark color="#1B2441">
          <v-icon class="header-icon title">sbf-person-icon</v-icon>
          <v-toolbar-title class="subheading" >{{dialogTitle}}</v-toolbar-title>
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-card-text class="pb-0">
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
              <v-flex xs12 md4 v-if="!isAuthUser" >
                  <v-text-field maxlength="12"
                  type="tel"
                    v-model="guestPhone"
                    :rules="[rules.required]"
                    :placeholder="guestPhoneNumberPlaceHolder"
                    autocomplete="off"/>
              </v-flex>
            <v-flex xs12>
                  <v-textarea 
                    :rows="isAuthUser? 1 : 3"
                    name="add-request-textarea"
                    :label="topicPlaceholder"
                    :rules="[rules.required, rules.maximumChars]"
                    v-model="tutorRequestText"/>
            </v-flex>

              <v-flex xs12 md6 v-if="!isAuthUser" >
                <v-combobox
                    @keyup="searchCourses"
                  flat
                  hide-no-data
                  :append-icon="''"
                  :menu-props="{contentClass:'courses-select-list'}"
                  v-model="tutorCourse"
                  :items="suggestsCourses"
                  :placeholder="coursePlaceholder"
                  :rules="[rules.required,rules.matchCourse]"/>
              </v-flex>
              <v-flex xs12 v-else>
                <v-combobox
                  @keyup="searchCourses"
                  flat
                  hide-no-data
                  :append-icon="''"
                  :menu-props="{contentClass:'courses-select-list'}"
                  v-model="tutorCourse"
                  :items="suggestsCourses"
                  :placeholder="coursePlaceholder"
                  :rules="[rules.required,rules.matchCourse]"/>
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
          <div class="pl-4">
            <vue-recaptcha v-if="!isAuthUser" 
              size="invisible"
              class="pb-3"
              :sitekey="siteKey"
              ref="recaptcha"
              @verify="onVerify"
              @expired="onExpired()">
            </vue-recaptcha>
          </div>
        <v-card-actions class="alignEnd pt-0">
        <div>
            <v-btn color="blue darken-1" :disabled="btnRequestLoading" flat @click="tutorRequestDialogClose()">{{btnClosePlaceholder}}</v-btn>
            <v-btn  color="#4452fc"
                   class="white--text"
                   :loading="btnRequestLoading" 
                   round depressed
                   @click="submit(!isAuthUser)">
                   {{btnSubmitPlaceholder}}
            </v-btn>
          </div>
        </v-card-actions>
      </v-card>
    </v-form>
  </transition>
</template>


<script src="./tutorRequest.js"></script>

<style lang="less" src="./tutorRequest.less"></style>