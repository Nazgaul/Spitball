<template>
  <transition name="fade">
    <v-form v-model="validRequestTutorForm" ref="tutorRequestForm">

    <v-card>
        <v-card-title><span class="headline" v-language:inner>tutorRequest_title</span></v-card-title>
        <v-card-text>
          <v-container grid-list-md>
            <v-layout wrap>

              <v-flex xs12 >
                  <v-text-field 
                    v-model="guestName"
                    :rules="[rules.required]"
                    :placeholder="guestNamePlaceHolder"
                    autocomplete="off"/>
              </v-flex>
              <v-flex xs12>
                  <v-text-field 
                    v-model="guestMail"
                    type="email"
                    :rules="[rules.required]"
                    :placeholder="guestEmailPlaceHolder"
                    autocomplete="off"/>
              </v-flex>
              
              <v-flex xs12>
                  <v-text-field 
                    v-model="guestPhone"
                    type="tel"
                    :rules="[rules.required]"
                    :placeholder="guestPhoneNumberPlaceHolder"
                    autocomplete="off"/>
              </v-flex>

              <v-flex xs12>
                  <v-textarea 
                    auto-grow rows="1" 
                    no-resize
                    name="add-request-textarea"
                    :label="topicPlaceholder"
                    :rules="[rules.required, rules.maximumChars]"
                    v-model="tutorRequestText"/>
            </v-flex>

              <v-flex xs12>
                  <v-autocomplete
                    flat
                    hide-no-data
                    hide-details
                    :append-icon="'sbf-arrow-down'"
                    :menu-props="{contentClass:'courses-select-list'}"
                    v-model="tutorCourse"
                    :items="getSelectedClasses"
                    :placeholder="coursePlaceholder"
                    :rules="[rules.required]"
                  ></v-autocomplete>

              </v-flex>

              <v-flex xs12><v-text-field :placeholder="universityPlaceHolder"/></v-flex>
            </v-layout>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" flat @click="tutorRequestDialogClose()" :loading="btnRequestLoading">{{btnClosePlaceholder}}</v-btn>
          <v-btn color="blue darken-1" flat @click="sendRequest()">{{btnSubmitPlaceholder}}</v-btn>
        </v-card-actions>
      </v-card>
</v-form>


    <!-- <v-form v-model="validRequestTutorForm" ref="tutorRequestForm">
      <v-layout column justify-start class="add-request-container">
        <v-layout
          shrink
          align-center
          justify-space-around
          class="request-header py-2"
          :class="[$vuetify.breakpoint.xsOnly ? 'px-4' : 'px-2']"
        >
          <v-flex>
            <span class="request-tutor-header-title">
              <v-icon class="header-icon mr-2">sbf-person-icon</v-icon>
              <span v-language:inner class="caption font-weight-bold">tutorRequest_title</span>
            </span>
          </v-flex>
          <v-flex class="text-xs-right">
            <button class="back-button" @click="tutorRequestDialogClose()">
              <v-icon right>sbf-close</v-icon>
            </button>
          </v-flex>
        </v-layout>
        <v-flex grow>
          <v-layout column class="full-height">
            <v-flex grow class="ma-2">
              <div class="request-textarea-container mt-3">
                <div class="request-textarea-upper-part">
                  <user-avatar :userImageUrl="userImageUrl" :user-name="userName"></user-avatar>
                  <v-textarea
                    solo
                    no-resize
                    flat
                    name="add-request-textarea"
                    :label="topicPlaceholder"
                    class="request-textarea elevation-0"
                    :rows="7"
                    :rules="[rules.required, rules.maximumChars]"
                    v-model="tutorRequestText"
                  ></v-textarea>
                </div>
                <div class="middle-part">
                  <div
                    class="request-thumbnails-part"
                    :class="{'show-tumbnails': uploadProp.uploadedFiles.length > 0}"
                  >
                    <div
                      v-for="thumbnailBox in [0,1,2,3]"
                      :key="thumbnailBox"
                      class="request-attachment-box horizontal-border vertical-border"
                    >
                      <div class="request-thumb-img-container">
                        <img
                          v-show="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].populated"
                          :src="uploadProp.populatedThumnbailBox['box_'+thumbnailBox].src"
                        >
                      </div>
                    </div>
                  </div>
                </div>
                <div class="request-textarea-lower-part pt-2" v-if="isAuthUser">
                  <div class="request-options-part">
                    <div class="request-select">
                      <v-select
                        :menu-props="{contentClass:'courses-select-list'}"
                        height="32"
                        v-model="tutorCourse"
                        :items="getSelectedClasses"
                        :placeholder="coursePlaceholder"
                        :rules="[rules.required]"
                        :append-icon="'sbf-arrow-down'"
                        outline
                      >
                        <template slot="no-data">
                          <div class="v-select-list v-card theme--light">
                            <div role="list" class="v-list theme--light">
                              <div role="listitem">
                                <div class="v-list__tile theme--light">
                                  <div class="v-list__tile__content">
                                    <div
                                      class="v-list__tile__title"
                                      v-language:inner
                                    >tutorRequest_no_course</div>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </div>
                        </template>
                      </v-select>
                    </div>
                  </div>
                </div>
              </div>
              <div v-if="!isAuthUser" class="mt-4 anonymous-extra">
                <v-text-field
                  v-model="guestName"
                  class="class-input"
                  solo
                  flat
                  height="40"
                  :rules="[rules.required]"
                  :placeholder="guestNamePlaceHolder"
                  autocomplete="off"
                  spellcheck="true"
                ></v-text-field>
                <v-text-field
                  v-model="guestMail"
                  class="class-input"
                  solo
                  flat
                  type="email"
                  :placeholder="guestEmailPlaceHolder"
                  autocomplete="off"
                  spellcheck="true"
                ></v-text-field>
                <v-text-field
                  v-model="guestPhone"
                  class="class-input"
                  solo
                  flat
                  type="tel"
                  :rules="[rules.required]"
                  :placeholder="guestPhoneNumberPlaceHolder"
                  autocomplete="off"
                  spellcheck="true"
                ></v-text-field>
              </div>
              <v-layout align-center justify-center class="mt-3 mb-3">
                <span
                  class="request-files-btn d-inline-flex align-center justify-center pl-2 pr-3 ml-3"
                >
                  <v-icon class="attach-icon mr-2">sbf-attach</v-icon>
                  <span v-language:inner>tutorRequest_btn_attachment</span>
                  
                </span>
              </v-layout>
            </v-flex>
            <v-layout
              shrink
              class="request-add-button-container pt-12 pb-3"
              align-center
              justify-center
            >
              <v-flex xs12 class="text-xs-center">
                <v-btn
                  :loading="btnRequestLoading"
                  class="request-add-button subheading font-weight-bold px-3"
                  @click="sendRequest()"
                >
                  <span v-language:inner>tutorRequest_btn_submit</span>
                </v-btn>
              </v-flex>
            </v-layout>
          </v-layout>
        </v-flex>
      </v-layout>
    </v-form>
     -->
  </transition>
</template>


<script src="./tutorRequest.js"></script>

<style lang="less" src="./tutorRequest.less"></style>