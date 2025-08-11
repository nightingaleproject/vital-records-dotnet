# NVSS API Test Scripts

This directory contains two Ruby scripts for testing the NVSS FHIR API, useful for activities like submitting and retrieving high volumes of data.

## Prerequisites and Setup

Before running these scripts, you need to complete a few setup steps.

1.  **Ruby and Gems:** Ensure you have Ruby installed. Then, install the required gems using Bundler:
    ```bash
    bundle install
    ```

2.  **Configuration File:** Create a file named `config.yml` in this directory. The scripts will not run without it. The file should contain your API credentials in the following format:
    ```yaml
    ---
    client_id: your_client_id
    client_secret: your_client_secret
    username: your_username
    password: your_password
    ```

3.  **CLI Tool (for `pull_and_process.rb`):** The `pull_and_process.rb` script depends on a .NET CLI tool to generate acknowledgements. You must build the `VRDR.CLI` project first, as the script looks for the compiled DLL at `../../projects/VRDR.CLI/bin/Debug/netcoreapp6.0/VRDR.CLI.dll`.

---

## Submitting Messages (`push.rb`)

The `push.rb` script submits one or more FHIR messages to the API. By default, it sends messages in parallel batches of 20 for high throughput.

### Usage

```bash
bundle exec ruby push.rb <jurisdictionID> <message-file-1> [message-file-2...] [--single] [--sequential]
```

### Arguments

*   `<jurisdictionID>`: **(Required)** The two-letter code for the jurisdiction you are submitting for (e.g., `MA`).
*   `<message-file-1>`: **(Required)** The path to one or more FHIR message files to submit.

### Optional Flags

*   `--single`: Submits each message in a separate API call instead of batching them in Bundles.
*   `--sequential`: Disables parallel processing and sends all messages or batches sequentially from a single process.

### Example

```bash
# Submit three messages for Massachusetts in a single batch
bundle exec ruby push.rb MA fhir/message1.json fhir/message2.json fhir/message3.json
```

---

## Pulling and Processing Messages (`pull_and_process.rb`)

The `pull_and_process.rb` script fetches new messages from the API for a given jurisdiction. It processes each message based on its type: saving it to a local file and, where appropriate, automatically generating and sending an acknowledgement message back to the API.

### Usage

```bash
bundle exec ruby pull_and_process.rb <jurisdictionID> [hours]
```

### Arguments

*   `<jurisdictionID>`: **(Required)** The two-letter code for the jurisdiction to check (e.g., `MA`).
*   `[hours]`: **(Optional)** An integer number of hours. If provided, the script will only fetch messages updated within that time frame (e.g., `24` for the last day). If omitted, it fetches all available messages.

### Behavior

*   **Acknowledgement Messages:** Saved as `<identifier>_submission_acknowledgement.json`.
*   **Status & Error Messages:** Saved locally. Extraction errors are automatically acknowledged.
*   **Coded Messages:** Saved locally (e.g., `..._cause_of_death_coding.json`). The script automatically generates an acknowledgement, saves it, and sends it to the API.

### Examples

```bash
# Pull all available messages for Massachusetts
bundle exec ruby pull_and_process.rb MA

# Pull messages for Texas that were updated in the last 12 hours
bundle exec ruby pull_and_process.rb TX 12
```
