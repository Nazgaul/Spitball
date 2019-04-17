/**
 * Created by thoryachev on 22.11.2018.
 */
(function (document, mpl) {

  // Cache DOM Nodes ---------------------------------------------------------------------------------------------------

  const form = document.getElementById('checkout-form');
  const submitButton = document.getElementById('submit-button');
  const cardProvider = document.getElementById('card-provider');
  const errorsMessagesContainer = document.getElementById('errors');

  const successQuery = document.querySelector('.first-example .success');
  const backFormButton = document.querySelector('.back-on-form1');

  const firstNameInput = document.getElementById('first-name-input');
  const lastNameInput = document.getElementById('last-name-input');
  const emailInput = document.getElementById('email-input');
  const phoneInput = document.getElementById('phone-input');
  const socialIdInput = document.getElementById('social-id-input');

  // Helpers -----------------------------------------------------------------------------------------------------------

  const errorsFromField = {};

  function tokenizationStarted() {
    form.classList.add('fadeOut');
    form.style.display = 'none';
    successQuery.style.display = 'block';
    successQuery.querySelector('.wrap-loading').style.display = 'block';
    successQuery.classList.add('fadeIn');
    submitButton.disabled = true;
    console.log('Tokenization started!');
  }

  function tokenizationFinished(error) {
    successQuery.querySelector('.wrap-loading').style.display = 'none';
    submitButton.disabled = false;
    console.log('Tokenization finished!');

    if(error) {
      console.error(error);

      const failedValidation = {
        field: PayMe.fields.NONE, isValid: false, message: ''
      };

      // Checking is tokenization processing error
      if(error.type && error.type === 'tokenize-error') {
        // Handle tokenization processing error
        const [ firstErrorMessage ] = Object.values(error.errors);
        failedValidation.message = firstErrorMessage;
      } else {
        // Handle other errors from PayMe
        failedValidation.message = error.message;
      }

      toggleValidationMessages(failedValidation);
    } else {
      firstNameInput.value = lastNameInput.value = emailInput.value = phoneInput.value = socialIdInput.value = '';
    }
  }

  function showErrors(errorsFromField, validationResult) {
    let lastElement = errorsFromField[Object.keys(errorsFromField).pop()];
    errorsMessagesContainer.classList.remove('fadeOutDown');
    if (!validationResult.message) {
      errorsMessagesContainer.innerText = lastElement;
    } else {
      errorsMessagesContainer.innerText = validationResult.message;
    }
  }

  function toggleValidationMessages(validationResult) {

    delete errorsFromField[PayMe.fields.NONE];

    if (validationResult.isValid) {
      errorsMessagesContainer.classList.remove('fadeInUp');
      errorsMessagesContainer.classList.add('fadeOutDown');
      delete errorsFromField[validationResult.field]; // delete error from the object that passed validation

      if (Object.keys(errorsFromField).length > 0) { // if the object still has errors - output them
        showErrors(errorsFromField, validationResult);
        errorsMessagesContainer.classList.remove('fadeOutDown');
        errorsMessagesContainer.classList.add('fadeInUp');
      }
    } else {
      errorsFromField[validationResult.field] = validationResult.message; // write errors to the object
      errorsMessagesContainer.classList.remove('fadeOutDown');
      errorsMessagesContainer.classList.add('fadeInUp');
      if (Object.keys(errorsFromField).length > 0) { // check if there is an error in the object
        showErrors(errorsFromField, validationResult); // and show its
      }
    }
  }

  function changeCardProviderIcon(cardVendor) {

    const vendorsToClasses = {
      'unknown': ['fas', 'fa-credit-card'],

      'amex': ['fab', 'fa-cc-amex'],
      'diners': ['fab', 'fa-cc-diners-club'],
      'jcb': ['fab', 'fa-cc-jcb'],
      'visa': ['fab', 'fa-cc-visa'],
      'mastercard': ['fab', 'fa-cc-mastercard'],
      'discover': ['fab', 'fa-cc-discover'],
    };

    for(let i = cardProvider.classList.length-1; i >= 0; i-- ){
        cardProvider.classList.remove(cardProvider.classList[i]);
    }
    let item = vendorsToClasses[cardVendor] || vendorsToClasses['unknown'];
    item.forEach( el => {
      cardProvider.classList.add(el);
    })
  }

  function addClass(fieldId, className) {
    document.getElementById(fieldId).classList.add(className);
  }

  function removeClass(fieldId, className) {
    document.getElementById(fieldId).classList.remove(className);
  }

  function showSuccessQuery(data) {
    successQuery.querySelector('.name').innerHTML = "<span>Name:</span> " + data.payerName;
    successQuery.querySelector('.email').innerHTML = "<span>Email:</span> " + data.payerEmail;
    successQuery.querySelector('.phone').innerHTML = "<span>Phone:</span> " + data.payerPhone;
    successQuery.querySelector('.socialId').innerHTML = "<span>Social Id:</span> " + data.payerSocialId;
    successQuery.querySelector('.token').innerHTML = "<span>Token:</span> " + data.token;
  }

  function runNativeFieldValidator(value, field, messages){
    const validator = PayMe.validators[field];
    const errors = validator.test(value);
    let message;
    if(errors && errors.required) {
      message = messages.required
    }
    if(errors && errors.invalid) {
      message = messages.invalid
    }

    return { isValid: !errors, field: field, message: message };
  }

  function createNativeFieldValidatorHandler(fieldName, messagesObject) {
    return function(ev) {
      const inputNode = this;
      const validation = runNativeFieldValidator(ev.target.value, fieldName, messagesObject);

      if (validation.isValid) {
        inputNode.classList.remove('invalid');
        inputNode.classList.add('valid');
      } else {
        inputNode.classList.remove('valid');
        inputNode.classList.add('invalid');
      }

      toggleValidationMessages(validation);
    }
  }

  // Misc --------------------------------------------------------------------------------------------------------------

  const allFieldsReady = [];

  const DEFAULT_SETTINGS = {
    styles: {
      base: {
        'font-size': '16px',
        '::placeholder': {'color': '#ACD7E4'}
      },
      invalid: {
        'color': '#FF0000',
      },
      valid: {
        'color': '#fff',
      },
    }
  };

  // Main --------------------------------------------------------------------------------------------------------------
  function init() {

    // Disable submit button until protected fields initialization
    submitButton.disabled = true;

    // Getting hosted fields integration manager
    PayMe.create(mpl, { testMode: true }).then((instance) => {

      const fields = instance.hostedFields();

      // Protected fields ------------------------------------------------------

      // Card Number
      const cardNumberSettings =  Object.assign({}, DEFAULT_SETTINGS, {
        placeholder: 'Credit Card Number',
        messages: {
          invalid: 'Bad credit card number',
          required: 'Field "Card Number" is mandatory'
        },
      });
      const cardNumber = fields.create(PayMe.fields.NUMBER, cardNumberSettings);
      allFieldsReady.push(
        cardNumber.mount('#card-number-container')
      );
      cardNumber.on('card-type-changed', ev => changeCardProviderIcon(ev.cardType));
      cardNumber.on('keyup', toggleValidationMessages);
      cardNumber.on('keyup', (e) => {
        if (e.isValid) {
          expiration.focus();
        }
        e.isEmpty ? removeClass('card-expiration-group', 'animate-card-option') : addClass('card-expiration-group', 'animate-card-option');
        e.isEmpty ? removeClass('card-cvv-group', 'animate-card-option') : addClass('card-cvv-group', 'animate-card-option');
      });

      // Expiry Date
      const expirationField = Object.assign({}, DEFAULT_SETTINGS, {
        messages: {
          invalid: 'Invalid Expiration',
          required: 'Field "Expiration" is mandatory'
        },
      });
      const expiration = fields.create(PayMe.fields.EXPIRATION, expirationField);
      allFieldsReady.push(
        expiration.mount('#card-expiration-container')
      );
      expiration.on('keyup', toggleValidationMessages);
      expiration.on('validity-changed', toggleValidationMessages);
      expiration.on('keyup', (e) => {
        if (e.isValid) {
          cvc.focus();
        }
      });

      // CVC/CVV
      const cvcField = Object.assign({}, DEFAULT_SETTINGS, {
        placeholder: 'CVV',
        messages: {
          invalid: 'Invalid CVV',
          required: 'Field "CVV" is mandatory'
        },
      });
      const cvc = fields.create(PayMe.fields.CVC, cvcField);
      allFieldsReady.push(
        cvc.mount('#card-cvv-container')
      );
      cvc.on('keyup', toggleValidationMessages);
      cvc.on('validity-changed', toggleValidationMessages);

      // AUX fields ------------------------------------------------------------

      // First Name
      const firstNameMessages = {
        invalid: 'Letters only for field "First name"', required: 'Field "First name" is mandatory'
      };
      firstNameInput.addEventListener(
        'keyup', createNativeFieldValidatorHandler(PayMe.fields.NAME_FIRST, firstNameMessages)
      );
      firstNameInput.addEventListener(
        'focus', createNativeFieldValidatorHandler(PayMe.fields.NAME_FIRST, firstNameMessages)
      );

      // Last Name
      const lastNameMessages = {
        invalid: 'Letters only for field "Last name"', required: 'Field "Last name" is mandatory'
      };
      lastNameInput.addEventListener(
        'keyup', createNativeFieldValidatorHandler(PayMe.fields.NAME_LAST, lastNameMessages)
      );
      lastNameInput.addEventListener(
        'focus', createNativeFieldValidatorHandler(PayMe.fields.NAME_LAST, lastNameMessages)
      );

      // Email
      const emailMessages = {
        invalid: 'Invalid Email', required: 'Field "Email" is mandatory'
      };
      emailInput.addEventListener(
        'keyup', createNativeFieldValidatorHandler(PayMe.fields.EMAIL, emailMessages)
      );
      emailInput.addEventListener(
        'focus', createNativeFieldValidatorHandler(PayMe.fields.EMAIL, emailMessages)
      );

      // Phone Number
      const phoneMessages = {
          invalid: 'Invalid Phone', required: 'Field "Phone" is mandatory'
      };
      phoneInput.addEventListener(
        'keyup', createNativeFieldValidatorHandler(PayMe.fields.PHONE, phoneMessages)
      );
      phoneInput.addEventListener(
        'focus', createNativeFieldValidatorHandler(PayMe.fields.PHONE, phoneMessages)
      );

      // Social Id
      const socialIdMessages = {
        invalid: 'Invalid Social Id', required: 'Field "Social Id" is mandatory'
      };
      socialIdInput.addEventListener(
        'keyup',  createNativeFieldValidatorHandler(PayMe.fields.SOCIAL_ID, socialIdMessages)
      );
      socialIdInput.addEventListener(
        'focus',  createNativeFieldValidatorHandler(PayMe.fields.SOCIAL_ID, socialIdMessages)
      );

      // Wait for fields initialization ----------------------------------------

      Promise.all(allFieldsReady).then(() => submitButton.disabled = false);

      // Form submission handler -----------------------------------------------

      const formSubmit = ev => {
        ev.preventDefault();

        const sale = {

          payerFirstName: firstNameInput.value,
          payerLastName: lastNameInput.value,
          payerEmail: emailInput.value,
          payerPhone: phoneInput.value,
          payerSocialId: socialIdInput.value,

          total: {
            label: '🚀 Rubber duck',
            amount: {
              currency: 'ILS',
              value: '55.00',
            }
          }
        };

        tokenizationStarted();
        toggleValidationMessages({ field: PayMe.fields.NONE, isValid: true});

        instance.tokenize(sale)
          .then(data => {
            console.log('Tokenization result::: ', data);
            showSuccessQuery(data);
            tokenizationFinished();
          })
          .catch(err => {
            alert('Tokenization failed');

            successQuery.style.display = 'none';
            form.style.display = 'block';
            form.classList.remove('fadeOut');
            tokenizationFinished(err);
          });
      };

      // Return and recreate handler -------------------------------------------

      const clickToBackOnForm = () => {
        successQuery.style.display = 'none';

        instance.teardown();

        form.removeEventListener('submit', formSubmit);
        backFormButton.removeEventListener('click', clickToBackOnForm);

        form.classList.remove('fadeOut');
        form.classList.add('fadeIn');
        form.style.display = 'block';
        init();
      };

      // Events binding --------------------------------------------------------

      form.addEventListener('submit', formSubmit);
      backFormButton.addEventListener('click', clickToBackOnForm);

    });
  }

  init();

})(document, 'MPL15546-31186SKB-53ES24ZG-WGVCBKO2');
